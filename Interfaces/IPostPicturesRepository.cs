using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Interfaces
{
    public interface IPostPicturesRepository
    {
        Task<post_pictures> Insert(post_pictures picture);
        Task<IEnumerable<post_pictures>> GetByPostId(int postId);
        Task<int> CountByPostId(int postId);
    }
}
