using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UGB.MVC.Aplicaciones.Seguras.Helper;
using UGB.MVC.Aplicaciones.Seguras.Interfaces;
using UGB.MVC.DTO.CommentsDTO;
using UGB.MVC.Entities;
using UGB.MVC.Helper;

namespace UGB.MVC.Controllers
{
    [ApiController]
    [Route("api/posts/{postId:int}/comments")]
    [Authorize]
    public class CommentsController(
        IValidator<CreateCommentDTO> createCommentDTOValidator,
        ICommentsRepository commentsRepository,
        IPostsRepository postsRepository) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> Create(int postId, [FromBody] CreateCommentDTO createCommentDTO)
        {
            var validation = createCommentDTOValidator.Validate(createCommentDTO);
            if(!validation.IsValid)
            {
                return BadRequest(validation.ToErrorResponse());
            }

            posts? post = await postsRepository.Get(postId);
            if(post == null)
            {
                return NotFound(new ErrorResponse
                {
                    Message = "La publicación no existe.",
                    StatusCode = 404
                });
            }

            int userId = (int)User.GetProperty("UserId", typeof(int));
            string userEmail = User.GetProperty(ClaimTypes.Email);

            comments comment = new comments
            {
                content = createCommentDTO.content,
                post_id = postId,
                user_id = userId,
                created_on = DateTime.UtcNow
            };

            comment = await commentsRepository.Insert(comment);

            CommentDTO commentDTO = MapToDTO(comment);
            commentDTO.authorEmail = userEmail;
            return Ok(commentDTO);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ListByPost(int postId)
        {
            var comments = await commentsRepository.GetByPostId(postId);
            return Ok(comments.Select(MapToDTO));
        }

        private static CommentDTO MapToDTO(comments comment)
        {
            return new CommentDTO
            {
                id = comment.id,
                content = comment.content,
                authorEmail = comment.user?.email ?? string.Empty,
                createdOn = comment.created_on
            };
        }
    }
}
