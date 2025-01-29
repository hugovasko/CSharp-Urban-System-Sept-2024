namespace UrbanSystem.Data.Models
{
    public class Comment
    {
        public Comment()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime AddedOn { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public Guid SuggestionId { get; set; }
        public Suggestion Suggestion { get; set; } = null!;
        
        public virtual ICollection<CommentRating> Ratings { get; set; } = new HashSet<CommentRating>();
    }
}
