using Microsoft.EntityFrameworkCore;
using UGB.MVC.Aplicaciones.Seguras.Entities;
using UGB.MVC.Aplicaciones.Seguras.Interfaces;
using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Repositories
{
    public class PostsRepository(StoreCTX ctx) : IPostsRepository
    {
        public async Task<posts> Insert(posts post)
        {
            ctx.posts.Add(post);
            await ctx.SaveChangesAsync();
            return post;
        }

        public async Task<posts?> Get(int id)
        {
            return await ctx.posts
                .Include(x => x.user)
                .FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<IEnumerable<posts>> GetAllPublished()
        {
            return await ctx.posts
                .Include(x => x.user)
                .Where(x => x.published)
                .OrderByDescending(x => x.created_on)
                .ToListAsync();
        }

        public async Task<posts> Update(posts post)
        {
            ctx.posts.Update(post);
            await ctx.SaveChangesAsync();
            return post;
        }

        public async Task Delete(posts post)
        {
            var relatedComments = ctx.comments.Where(x => x.post_id == post.id);
            ctx.comments.RemoveRange(relatedComments);

            var relatedPictures = ctx.post_pictures.Where(x => x.post_id == post.id);
            ctx.post_pictures.RemoveRange(relatedPictures);

            ctx.posts.Remove(post);
            await ctx.SaveChangesAsync();
        }
    }
}
