using FluentValidation;
using UGB.MVC.DTO.UsersDTO;

namespace UGB.MVC.Validations.UsersValidation
{
    public class UpdatePersonalDataDTOValidation : AbstractValidator<UpdatePersonalDataDTO>
    {
        public UpdatePersonalDataDTOValidation()
        {
            RuleFor(x => x.firstName)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(30).WithMessage("El nombre no debe exceder los 30 caracteres.")
                .Matches(@"^[\p{L}\s]+$").WithMessage("El nombre no debe contener números ni caracteres especiales.");

            RuleFor(x => x.lastName)
                .NotEmpty().WithMessage("El apellido es requerido.")
                .MaximumLength(30).WithMessage("El apellido no debe exceder los 30 caracteres.")
                .Matches(@"^[\p{L}\s]+$").WithMessage("El apellido no debe contener números ni caracteres especiales.");

            RuleFor(x => x.address)
                .NotEmpty().WithMessage("La dirección es requerida.")
                .MaximumLength(200).WithMessage("La dirección no debe exceder los 200 caracteres.");

            RuleFor(x => x.birthDate)
                .NotEmpty().WithMessage("La fecha de nacimiento es requerida.")
                .LessThan(DateTime.UtcNow).WithMessage("La fecha de nacimiento debe ser una fecha pasada.");

            RuleFor(x => x.dui)
                .NotEmpty().WithMessage("El número de DUI es requerido.")
                .Matches(@"^\d{8}-\d$").WithMessage("El DUI debe tener el formato 00000000-0.")
                .NotEqual("00000000-0").WithMessage("El número de DUI ingresado no es válido.");
        }
    }
}
