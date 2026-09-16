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
