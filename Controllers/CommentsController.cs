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
        private const string AdministratorRole = "Administrator";
        private const int MaxCommentsPerDay = 5;

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

            DateTime startOfDayUtc = DateTime.UtcNow.Date;

            int commentsToday = await commentsRepository.CountByUserSince(userId, startOfDayUtc);
            if(commentsToday >= MaxCommentsPerDay)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = $"Ha alcanzado el límite de {MaxCommentsPerDay} comentarios por día.",
                    StatusCode = 400
                });
            }

            bool alreadyCommentedToday = await commentsRepository.ExistsByUserAndPostSince(userId, postId, startOfDayUtc);
            if(alreadyCommentedToday)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Ya ha comentado esta publicación hoy.",
                    StatusCode = 400
                });
            }

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

        [HttpPut("{commentId:int}")]
        public async Task<ActionResult> Update(int postId, int commentId, [FromBody] CreateCommentDTO updateCommentDTO)
        {
            var validation = createCommentDTOValidator.Validate(updateCommentDTO);
            if(!validation.IsValid)
            {
                return BadRequest(validation.ToErrorResponse());
            }

            comments? comment = await commentsRepository.Get(commentId);
            if(comment == null || comment.post_id != postId)
            {
                return NotFound(new ErrorResponse
                {
                    Message = "El comentario no existe.",
                    StatusCode = 404
                });
            }

            int userId = (int)User.GetProperty("UserId", typeof(int));
            if(comment.user_id != userId)
            {
                return StatusCode(403, new ErrorResponse
                {
                    Message = "No tiene permiso para modificar este comentario.",
                    StatusCode = 403
                });
            }

            comment.content = updateCommentDTO.content;
            comment = await commentsRepository.Update(comment);

            return Ok(MapToDTO(comment));
        }

        [HttpDelete("{commentId:int}")]
        public async Task<ActionResult> Delete(int postId, int commentId)
        {
            comments? comment = await commentsRepository.Get(commentId);
            if(comment == null || comment.post_id != postId)
            {
                return NotFound(new ErrorResponse
                {
                    Message = "El comentario no existe.",
                    StatusCode = 404
                });
            }

            int userId = (int)User.GetProperty("UserId", typeof(int));
            if(comment.user_id != userId && !User.IsInRole(AdministratorRole))
            {
                return StatusCode(403, new ErrorResponse
                {
                    Message = "No tiene permiso para eliminar este comentario.",
                    StatusCode = 403
                });
            }

            await commentsRepository.Delete(comment);
            return NoContent();
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
