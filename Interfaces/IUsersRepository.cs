using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Interfaces
{
    public interface IUsersRepository
    {
        Task<users> Insert(users user);
        Task<users?> GetByEmail(string email);
        Task<users?> Get(int id);
        Task<IEnumerable<users>> GetAll();
        Task<users?> GetByConfirmationToken(Guid token);
        Task<users> Update(users user);
        Task<users?> GetByDui(string dui);
        Task Delete(users user);
    }
}
