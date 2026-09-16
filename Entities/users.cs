namespace UGB.MVC.Entities
{
    public class users
    {
        public int id { get; set; }
        public string email { get; set; } = string.Empty;
        public string salt { get; set; } = string.Empty;
        public string password_hash { get; set; } = string.Empty;
        public DateTime created_on { get; set; }
        public ICollection<user_roles> user_roles { get; set; } = new List<user_roles>();
        public bool email_confirmed { get; set; }
        public Guid? email_confirmation_token { get; set; }
        public int failed_login_attempts { get; set; }
        public DateTime? locked_until { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? address { get; set; }
        public DateTime? birth_date { get; set; }
        public string? dui { get; set; }
        public string? photo_path { get; set; }
    }
}
