namespace UrbanSystem.Data.Models;

public class CommentRating
{
    public CommentRating()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public Guid CommentId { get; set; }
    public virtual Comment Comment { get; set; } = null!;
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    public bool IsLike { get; set; }
    public DateTime RatedOn { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}