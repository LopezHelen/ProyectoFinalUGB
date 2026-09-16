namespace UGB.MVC.DTO.UsersDTO
{
    public class UpdatePersonalDataDTO
    {
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public DateTime birthDate { get; set; }
        public string dui { get; set; } = string.Empty;
    }
}
