using FluentValidation;
using UGB.MVC.DTO.CommentsDTO;

namespace UGB.MVC.Validations.CommentsValidation
{
    public class CreateCommentDTOValidation : AbstractValidator<CreateCommentDTO>
    {
        public CreateCommentDTOValidation()
        {
            RuleFor(x => x.content)
                .NotEmpty().WithMessage("El comentario no debe estar vacío.")
                .Length(1, 200).WithMessage("El comentario no debe exceder los 200 caracteres.")
                .Must(x => !x.Contains('<') && !x.Contains('>')).WithMessage("El comentario no debe contener etiquetas HTML.");
        }
    }
}
