using FluentValidation;
using UGB.MVC.Aplicaciones.Seguras.DTO.UsersDTO;
using UGB.MVC.Aplicaciones.Seguras.Helper;

namespace UGB.MVC.Aplicaciones.Seguras.Validations.UsersValidation
{
    public class LoginUserDTOValidation : AbstractValidator<LoginUserDTO>
    {
        public LoginUserDTOValidation()
        {
            RuleFor(x => x.email)
                .NotNullOrWhiteSpace().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no es válido.");

            RuleFor(x => x.password)
                .NotNullOrWhiteSpace().WithMessage("La contraseña es obligatoria.");
        }
    }
}