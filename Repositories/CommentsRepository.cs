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

        public async Task<comments?> Get(int id)
        {
            return await ctx.comments
                .Include(x => x.user)
                .FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<IEnumerable<comments>> GetByPostId(int postId)
        {
            return await ctx.comments
                .Include(x => x.user)
                .Where(x => x.post_id == postId)
                .OrderBy(x => x.created_on)
                .ToListAsync();
        }

        public async Task<comments> Update(comments comment)
        {
            ctx.comments.Update(comment);
            await ctx.SaveChangesAsync();
            return comment;
        }

        public async Task Delete(comments comment)
        {
            ctx.comments.Remove(comment);
            await ctx.SaveChangesAsync();
        }

        public async Task<int> CountByUserSince(int userId, DateTime sinceUtc)
        {
            return await ctx.comments
                .CountAsync(x => x.user_id == userId && x.created_on >= sinceUtc);
        }

        public async Task<bool> ExistsByUserAndPostSince(int userId, int postId, DateTime sinceUtc)
        {
            return await ctx.comments
                .AnyAsync(x => x.user_id == userId && x.post_id == postId && x.created_on >= sinceUtc);
        }
    }
}
