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
                .Length(3, 100).WithMessage("El título debe tener entre 3 y 100 caracteres.");

            RuleFor(x => x.content)
                .NotEmpty().WithMessage("El contenido es requerido.")
                .Length(10, 5000).WithMessage("El contenido debe tener entre 10 y 5000 caracteres.");
        }
    }
}
