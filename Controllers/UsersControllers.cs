using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UGB.MVC.Aplicaciones.Seguras.Helper;
using UGB.MVC.Aplicaciones.Seguras.Interfaces;
using UGB.MVC.DTO.UsersDTO;
using UGB.MVC.Entities;
using UGB.MVC.Helper;
using UGB.MVC.Interfaces;
using UGB.MVC.Mapper;

namespace UGB.MVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController(
        IValidator<CreateUserDTO> createUserDTOValidator,
        IValidator<UpdatePersonalDataDTO> updatePersonalDataDTOValidator,
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository,
        IEmailService emailService,
        IWebHostEnvironment webHostEnvironment) : ControllerBase
    {
        private const string DefaultRole = "User";
        private const string AdministratorRole = "Administrator";

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Create([FromBody] CreateUserDTO createUserDTO)
        {
            var validation = createUserDTOValidator.Validate(createUserDTO);
            if(!validation.IsValid)
            {
                return BadRequest(validation.ToErrorResponse());
            }

            if(await usersRepository.GetByEmail(createUserDTO.email) != null)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Este correo ya se encuentra registrado.",
                    StatusCode = 400
                });
            }

            HashedPassword hashedPassword = HashHelper.Hash(createUserDTO.password);

            users user = CustomMapper<users>.Map(createUserDTO);
            user.password_hash = hashedPassword.Password;
            user.salt = hashedPassword.Salt;
            user.created_on = DateTime.UtcNow;
            user.email_confirmed = false;
            user.email_confirmation_token = Guid.NewGuid();

            user = await usersRepository.Insert(user);

            roles defaultRole = await rolesRepository.GetByName(DefaultRole)
                ?? throw new InvalidOperationException("No se encontró el rol predeterminado.");

            await rolesRepository.AssignRoleToUser(user.id, defaultRole.id);

            string confirmUrl = $"{Request.Scheme}://{Request.Host}/api/Users/confirm-email?token={user.email_confirmation_token}";
            await emailService.SendMail(new Email
            {
                To = user.email,
                Subject = "Confirma tu correo electrónico",
                Body = $"<p>Gracias por registrarte. Para activar tu cuenta confirma tu correo haciendo clic en el siguiente enlace:</p><p><a href=\"{confirmUrl}\">{confirmUrl}</a></p>"
            });

            return Ok(CustomMapper<UserDTO>.Map(user));
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail([FromQuery] Guid token)
        {
            users? user = await usersRepository.GetByConfirmationToken(token);
            if(user == null)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "El enlace de confirmación no es válido.",
                    StatusCode = 400
                });
            }

            user.email_confirmed = true;
            user.email_confirmation_token = null;
            await usersRepository.Update(user);

            return Ok(new { Message = "Correo confirmado correctamente. Ya puede iniciar sesión." });
        }

        [Authorize(Roles = AdministratorRole)]
        [HttpGet("all")]
        public async Task<IActionResult> ListAll()
        {
            return Ok(CustomMapper<UserDTO>.Map(await usersRepository.GetAll()));
        }

        [Authorize(Roles = AdministratorRole)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            users? user = await usersRepository.Get(id);
            if(user == null)
            {
                return NotFound(new ErrorResponse
                {
                    Message = "El usuario no existe.",
                    StatusCode = 404
                });
            }

            await usersRepository.Delete(user);
            return NoContent();
        }

        [HttpGet("me")]
        public async Task<IActionResult> ShowAuthenticated()
        {
            string email = User.GetProperty(ClaimTypes.Email);
            users? user = await usersRepository.GetByEmail(email);

            if(user == null)
            {
                return Unauthorized();
            }

            return Ok(CustomMapper<UserProfileDTO>.Map(user));
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdatePersonalDataDTO updatePersonalDataDTO)
        {
            var validation = updatePersonalDataDTOValidator.Validate(updatePersonalDataDTO);
            if(!validation.IsValid)
            {
                return BadRequest(validation.ToErrorResponse());
            }

            string email = User.GetProperty(ClaimTypes.Email);
            users? user = await usersRepository.GetByEmail(email);
            if(user == null)
            {
                return Unauthorized();
            }

            users? duiOwner = await usersRepository.GetByDui(updatePersonalDataDTO.dui);
            if(duiOwner != null && duiOwner.id != user.id)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Ya existe otro usuario registrado con ese número de DUI.",
                    StatusCode = 400
                });
            }

            user.first_name = updatePersonalDataDTO.firstName;
            user.last_name = updatePersonalDataDTO.lastName;
            user.address = updatePersonalDataDTO.address;
            user.birth_date = updatePersonalDataDTO.birthDate;
            user.dui = updatePersonalDataDTO.dui;

            await usersRepository.Update(user);
            return Ok(CustomMapper<UserProfileDTO>.Map(user));
        }

        [HttpPost("me")]
        public async Task<IActionResult> UploadPhoto([FromForm] IFormFile photo)
        {
            if(photo == null || photo.Length == 0)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Debe adjuntar un archivo de imagen.",
                    StatusCode = 400
                });
            }

            if(photo.Length > ImageHelper.MaxSizeBytes)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "La fotografía no debe superar los 2 MB.",
                    StatusCode = 400
                });
            }

            byte[] photoBytes;
            using(var memoryStream = new MemoryStream())
            {
                await photo.CopyToAsync(memoryStream);
                photoBytes = memoryStream.ToArray();
            }

            if(!ImageHelper.IsSupportedImage(photoBytes, out string extension))
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "La fotografía debe ser una imagen válida (JPG o PNG).",
                    StatusCode = 400
                });
            }

            string email = User.GetProperty(ClaimTypes.Email);
            users? user = await usersRepository.GetByEmail(email);
            if(user == null)
            {
                return Unauthorized();
            }

            string userFolder = Path.Combine(
                webHostEnvironment.WebRootPath,
                "uploads",
                "users",
                user.id.ToString());

            Directory.CreateDirectory(userFolder);

            foreach(string oldFile in Directory.GetFiles(userFolder, "photo.*"))
            {
                System.IO.File.Delete(oldFile);
            }

            string fileName = "photo" + extension;
            await System.IO.File.WriteAllBytesAsync(Path.Combine(userFolder, fileName), photoBytes);

            user.photo_path = $"/uploads/users/{user.id}/{fileName}";
            await usersRepository.Update(user);

            return Ok(CustomMapper<UserProfileDTO>.Map(user));
        }
    }
}
