namespace UGB.MVC.Entities
{
    public class user_roles
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public users user { get; set; } = null!;
        public int role_id { get; set; }
        public roles role { get; set; } = null!;
    }
}
