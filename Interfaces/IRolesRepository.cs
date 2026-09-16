using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Interfaces
{
    public interface IRolesRepository
    {
        Task<roles?> GetByName(string name);
        Task AssignRoleToUser(int userId, int roleId);
        Task<IEnumerable<string>> GetRoleNamesByUserId(int userId);
    }
}
