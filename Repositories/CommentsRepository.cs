using Microsoft.EntityFrameworkCore;
using UGB.MVC.Aplicaciones.Seguras.Entities;
using UGB.MVC.Aplicaciones.Seguras.Interfaces;
using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Repositories
{
    public class CommentsRepository(StoreCTX ctx) : ICommentsRepository
    {
        public async Task<comments> Insert(comments comment)
        {
            ctx.comments.Add(comment);
            await ctx.SaveChangesAsync();
            return comment;
        }

        public async Task<IEnumerable<comments>> GetByPostId(int postId)
        {
            return await ctx.comments
                .Include(x => x.user)
                .Where(x => x.post_id == postId)
                .OrderBy(x => x.created_on)
                .ToListAsync();
        }
    }
}
