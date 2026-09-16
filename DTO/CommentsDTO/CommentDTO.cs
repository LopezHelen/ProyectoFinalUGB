namespace UGB.MVC.DTO.CommentsDTO
{
    public class CommentDTO
    {
        public int id { get; set; }
        public string content { get; set; } = string.Empty;
        public string authorEmail { get; set; } = string.Empty;
        public DateTime? createdOn { get; set; }
    }
}
