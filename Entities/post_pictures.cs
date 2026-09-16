namespace UGB.MVC.Entities
{
    public class post_pictures
    {
        public int id { get; set; }
        public int? post_id { get; set; }
        public posts? post { get; set; }
        public string file_original_name { get; set; } = string.Empty;
        public string file_name { get; set; } = string.Empty;
        public string file_hash { get; set; } = string.Empty;
        public DateTime? created_on { get; set; }
    }
}
