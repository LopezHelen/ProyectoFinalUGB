using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UGB.MVC.Aplicaciones.Seguras.DTO.UsersDTO;
using UGB.MVC.Aplicaciones.Seguras.Helper;
using UGB.MVC.Aplicaciones.Seguras.Interfaces;
using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class LoginController(
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository,
        IValidator<LoginUserDTO> loginUserDTOValidator,
        ITokenService tokenService) : ControllerBase
    {
        private const int MaxFailedAttempts = 3;
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate([FromBody] LoginUserDTO loginUserDTO)
        {
            var validation = loginUserDTOValidator.Validate(loginUserDTO);
            if(!validation.IsValid)
            {
                return BadRequest(validation.ToErrorResponse());
            }

            users? user = await usersRepository.GetByEmail(loginUserDTO.email);
            if(user == null)
            {
                return InvalidCredentials();
            }

            if(user.locked_until.HasValue)
            {
                if(user.locked_until.Value > DateTime.UtcNow)
                {
                    return InvalidCredentials();
                }

                user.locked_until = null;
                user.failed_login_attempts = 0;
            }

            bool validCredentials = HashHelper.CheckHash(
                loginUserDTO.password,
                user.password_hash,
                user.salt);

            if(!validCredentials)
            {
                user.failed_login_attempts++;
                if(user.failed_login_attempts >= MaxFailedAttempts)
                {
                    user.locked_until = DateTime.UtcNow.Add(LockoutDuration);
                }

                await usersRepository.Update(user);
                return InvalidCredentials();
            }

            if(user.failed_login_attempts > 0 || user.locked_until.HasValue)
            {
                user.failed_login_attempts = 0;
                user.locked_until = null;
                await usersRepository.Update(user);
            }

            if(!user.email_confirmed)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Debe confirmar su correo electrónico antes de iniciar sesión.",
                    StatusCode = 400
                });
            }

            var roleNames = await rolesRepository.GetRoleNamesByUserId(user.id);
            string token = tokenService.GenerateToken(user, roleNames);

            return Ok(new { token });
        }

        private ActionResult InvalidCredentials()
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Correo o contraseña incorrectos.",
                StatusCode = 400
            });
        }
    }
}
