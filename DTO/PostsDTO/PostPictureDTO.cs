namespace UGB.MVC.DTO.PostsDTO
{
    public class PostPictureDTO
    {
        public int id { get; set; }
        public string fileOriginalName { get; set; } = string.Empty;
        public string fileName { get; set; } = string.Empty;
        public string fileHash { get; set; } = string.Empty;
        public string url { get; set; } = string.Empty;
        public DateTime? createdOn { get; set; }
    }
}
