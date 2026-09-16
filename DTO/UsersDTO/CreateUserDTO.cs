namespace UGB.MVC.DTO.UsersDTO
{
    public class CreateUserDTO
    {
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string passwordConfirm { get; set; } = string.Empty;
    }
}
