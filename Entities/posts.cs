namespace UGB.MVC.Entities
{
    public class posts
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public int? user_id { get; set; }
        public users? user { get; set; }
        public bool published { get; set; } = true;
        public DateTime? created_on { get; set; }
        public ICollection<comments> comments { get; set; } = new List<comments>();
        public ICollection<post_pictures> post_pictures { get; set; } = new List<post_pictures>();
    }
}
