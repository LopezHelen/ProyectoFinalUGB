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
    [Route("api/posts/{postId:int}/pictures")]
    [Authorize]
    public class PostPicturesController(
        IPostPicturesRepository postPicturesRepository,
        IPostsRepository postsRepository,
        IWebHostEnvironment webHostEnvironment) : ControllerBase
    {
        private const int MaxPicturesPerPost = 5;

        [HttpPost]
        public async Task<ActionResult> Upload(int postId, [FromForm] IFormFile file)
        {
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
            if(post.user_id != userId)
            {
                return StatusCode(403, new ErrorResponse
                {
                    Message = "No tiene permiso para adjuntar imágenes a esta publicación.",
                    StatusCode = 403
                });
            }

            if(file == null || file.Length == 0)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Debe adjuntar un archivo de imagen.",
                    StatusCode = 400
                });
            }

            int existingCount = await postPicturesRepository.CountByPostId(postId);
            if(existingCount >= MaxPicturesPerPost)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = $"Un post admite un máximo de {MaxPicturesPerPost} imágenes.",
                    StatusCode = 400
                });
            }

            if(!ImageHelper.HasAllowedExtension(file.FileName))
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Solo se permiten archivos con extensión .jpg o .png.",
                    StatusCode = 400
                });
            }

            if(file.Length > ImageHelper.MaxSizeBytes)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Cada imagen no debe superar los 2 MB.",
                    StatusCode = 400
                });
            }

            byte[] fileBytes;
            using(var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            if(!ImageHelper.IsSupportedImage(fileBytes, out string extension))
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "El archivo no es una imagen JPG o PNG válida.",
                    StatusCode = 400
                });
            }

            string originalName = Path.GetFileName(file.FileName);
            string cleanFileName = ImageHelper.SanitizeFileName(originalName, extension);
            string fileHash = ImageHelper.ComputeSha256(fileBytes);

            post_pictures picture = new post_pictures
            {
                post_id = postId,
                file_original_name = originalName.Length > 50 ? originalName[..50] : originalName,
                file_name = cleanFileName,
                file_hash = fileHash,
                created_on = DateTime.UtcNow
            };

            picture = await postPicturesRepository.Insert(picture);

            string diskFileName = $"{picture.id}_{cleanFileName}";
            string folder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "posts", postId.ToString());
            Directory.CreateDirectory(folder);
            await System.IO.File.WriteAllBytesAsync(Path.Combine(folder, diskFileName), fileBytes);

            return Ok(MapToDTO(picture, postId));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ListByPost(int postId)
        {
            var pictures = await postPicturesRepository.GetByPostId(postId);
            return Ok(pictures.Select(p => MapToDTO(p, postId)));
        }

        private static PostPictureDTO MapToDTO(post_pictures picture, int postId)
        {
            return new PostPictureDTO
            {
                id = picture.id,
                fileOriginalName = picture.file_original_name,
                fileName = picture.file_name,
                fileHash = picture.file_hash,
                url = $"/uploads/posts/{postId}/{picture.id}_{picture.file_name}",
                createdOn = picture.created_on
            };
        }
    }
}
