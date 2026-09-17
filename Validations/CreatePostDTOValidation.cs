using FluentValidation;
using UGB.MVC.DTO.PostsDTO;

namespace UGB.MVC.Validations.PostsValidation
{
    public class CreatePostDTOValidation : AbstractValidator<CreatePostDTO>
    {
        public CreatePostDTOValidation()
        {
            RuleFor(x => x.title)
                .NotEmpty().WithMessage("El título es requerido.")
                .Length(3, 100).WithMessage("El título debe tener entre 3 y 100 caracteres.")
                .Must(NotContainHtml).WithMessage("El título no debe contener etiquetas HTML.");

            RuleFor(x => x.content)
                .NotEmpty().WithMessage("El contenido es requerido.")
                .Length(10, 5000).WithMessage("El contenido debe tener entre 10 y 5000 caracteres.")
                .Must(NotContainHtml).WithMessage("El contenido no debe contener etiquetas HTML.");
        }

        private static bool NotContainHtml(string value)
        {
            return !value.Contains('<') && !value.Contains('>');
        }
    }
}
