namespace UGB.MVC.DTO.PostsDTO
{
    public class PostDTO
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public string authorEmail { get; set; } = string.Empty;
        public bool published { get; set; }
        public DateTime? createdOn { get; set; }
    }
}
