using UGB.MVC.Helper;

namespace UGB.MVC.Interfaces
{
    public interface IEmailService
    {
        Task SendMail(Email emailData);
    }
}