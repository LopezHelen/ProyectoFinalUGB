using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Interfaces
{
    public interface ICommentsRepository
    {
        Task<comments> Insert(comments comment);
        Task<comments?> Get(int id);
        Task<IEnumerable<comments>> GetByPostId(int postId);
        Task<comments> Update(comments comment);
        Task Delete(comments comment);
        Task<int> CountByUserSince(int userId, DateTime sinceUtc);
        Task<bool> ExistsByUserAndPostSince(int userId, int postId, DateTime sinceUtc);
    }
}
