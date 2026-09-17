using FluentValidation;
using UGB.MVC.DTO.UsersDTO;

namespace UGB.MVC.Validations.UsersValidation
{
    public class CreateUserDTOValidation : AbstractValidator<CreateUserDTO>
    {
        public CreateUserDTOValidation()
        {
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("El correo es requerido.")
                .EmailAddress().WithMessage("Ingrese un correo válido.");

            RuleFor(x => x.password)
                .NotEmpty().WithMessage("La contraseña no debe estar vacía.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .MaximumLength(64).WithMessage("La contraseña no debe exceder los 64 caracteres.")
                .Matches(@"[A-Za-z]").WithMessage("La contraseña debe contener al menos una letra (mayúscula o minúscula).")
                .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número.");

            RuleFor(x => x.passwordConfirm)
                .NotEmpty().WithMessage("Debe confirmar la contraseña.")
                .Equal(x => x.password).WithMessage("Las contraseñas no coinciden.");
        }
    }
}
