using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Interfaces
{
    public interface IPostsRepository
    {
        Task<posts> Insert(posts post);
        Task<posts?> Get(int id);
        Task<IEnumerable<posts>> GetAllPublished();
        Task<posts> Update(posts post);
        Task Delete(posts post);
    }
}
