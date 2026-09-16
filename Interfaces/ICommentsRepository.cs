using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Interfaces
{
    public interface ICommentsRepository
    {
        Task<comments> Insert(comments comment);
        Task<IEnumerable<comments>> GetByPostId(int postId);
    }
}
