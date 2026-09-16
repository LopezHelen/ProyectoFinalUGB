namespace UGB.MVC.Entities
{
    public class comments
    {
        public int id { get; set; }
        public int? user_id { get; set; }
        public users? user { get; set; }
        public int? post_id { get; set; }
        public posts? post { get; set; }
        public string content { get; set; } = string.Empty;
        public DateTime? created_on { get; set; }
    }
}
