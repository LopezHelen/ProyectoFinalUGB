namespace UGB.MVC.DTO.UsersDTO
{
    public class UserProfileDTO
    {
        public int id { get; set; }
        public string email { get; set; } = string.Empty;
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? address { get; set; }
        public DateTime? birthDate { get; set; }
        public string? dui { get; set; }
        public string? photoUrl { get; set; }
    }
}
