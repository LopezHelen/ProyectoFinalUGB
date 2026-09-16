namespace UGB.MVC.Entities
{
    public class roles
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public ICollection<user_roles> user_roles { get; set; } = new List<user_roles>();
    }
}
