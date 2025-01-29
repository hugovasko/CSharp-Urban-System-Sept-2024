namespace UrbanSystem.Web.ViewModels
{
    public class CommentViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime AddedOn { get; set; }
        public string UserName { get; set; } = null!;
        public int VoteCount { get; set; }
        
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}