using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UGB.MVC.Aplicaciones.Seguras.Helper;
using UGB.MVC.Aplicaciones.Seguras.Interfaces;
using UGB.MVC.DTO.PostsDTO;
using UGB.MVC.Entities;
using UGB.MVC.Helper;

namespace UGB.MVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostsController(
        IValidator<CreatePostDTO> createPostDTOValidator,
        IPostsRepository postsRepository) : ControllerBase
    {
        private const string AdministratorRole = "Administrator";

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreatePostDTO createPostDTO)
        {
            var validation = createPostDTOValidator.Validate(createPostDTO);
            if(!validation.IsValid)
            {
                return BadRequest(validation.ToErrorResponse());
            }

            int userId = (int)User.GetProperty("UserId", typeof(int));
            string userEmail = User.GetProperty(ClaimTypes.Email);

            posts post = new posts
            {
                title = createPostDTO.title,
                content = createPostDTO.content,
                user_id = userId,
                created_on = DateTime.UtcNow
            };

            post = await postsRepository.Insert(post);

            PostDTO postDTO = MapToDTO(post);
            postDTO.authorEmail = userEmail;
            return Ok(postDTO);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ListAll()
        {
            var posts = await postsRepository.GetAllPublished();
            return Ok(posts.Select(MapToDTO));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult> Get(int id)
        {
            posts? post = await postsRepository.Get(id);
            if(post == null)
            {
                return NotFound(new ErrorResponse
                {
                    Message = "La publicación no existe.",
                    StatusCode = 404
                });
            }

            return Ok(MapToDTO(post));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreatePostDTO updatePostDTO)
        {
            var validation = createPostDTOValidator.Validate(updatePostDTO);
            if(!validation.IsValid)
            {
                return BadRequest(validation.ToErrorResponse());
            }

            posts? post = await postsRepository.Get(id);
            if(post == null)
            {
                return NotFound(new ErrorResponse
                {
                    Message = "La publicación no existe.",
                    StatusCode = 404
                });
            }

            int userId = (int)User.GetProperty("UserId", typeof(int));
            if(post.user_id != userId)
            {
                return StatusCode(403, new ErrorResponse
                {
                    Message = "No tiene permiso para modificar esta publicación.",
                    StatusCode = 403
                });
            }

            post.title = updatePostDTO.title;
            post.content = updatePostDTO.content;
            post = await postsRepository.Update(post);

            return Ok(MapToDTO(post));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            posts? post = await postsRepository.Get(id);
            if(post == null)
            {
                return NotFound(new ErrorResponse
                {
                    Message = "La publicación no existe.",
                    StatusCode = 404
                });
            }

            int userId = (int)User.GetProperty("UserId", typeof(int));
            if(post.user_id != userId && !User.IsInRole(AdministratorRole))
            {
                return StatusCode(403, new ErrorResponse
                {
                    Message = "No tiene permiso para eliminar esta publicación.",
                    StatusCode = 403
                });
            }

            await postsRepository.Delete(post);
            return NoContent();
        }

        private static PostDTO MapToDTO(posts post)
        {
            return new PostDTO
            {
                id = post.id,
                title = post.title,
                content = post.content,
                authorEmail = post.user?.email ?? string.Empty,
                published = post.published,
                createdOn = post.created_on
            };
        }
    }
}
