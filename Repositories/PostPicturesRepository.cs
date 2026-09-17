using Microsoft.EntityFrameworkCore;
using UGB.MVC.Aplicaciones.Seguras.Entities;
using UGB.MVC.Aplicaciones.Seguras.Interfaces;
using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Repositories
{
    public class PostPicturesRepository(StoreCTX ctx) : IPostPicturesRepository
    {
        public async Task<post_pictures> Insert(post_pictures picture)
        {
            ctx.post_pictures.Add(picture);
            await ctx.SaveChangesAsync();
            return picture;
        }

        public async Task<IEnumerable<post_pictures>> GetByPostId(int postId)
        {
            return await ctx.post_pictures
                .Where(x => x.post_id == postId)
                .OrderBy(x => x.created_on)
                .ToListAsync();
        }

        public async Task<int> CountByPostId(int postId)
        {
            return await ctx.post_pictures.CountAsync(x => x.post_id == postId);
        }
    }
}
