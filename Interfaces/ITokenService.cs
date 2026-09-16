using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(users user, IEnumerable<string> roleNames);
    }
}
