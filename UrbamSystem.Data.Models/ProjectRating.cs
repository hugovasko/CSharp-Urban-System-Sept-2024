namespace UrbanSystem.Data.Models;

// UrbanSystem.Data/Models/ProjectRating.cs
public class ProjectRating
{
    public ProjectRating()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public virtual Project Project { get; set; } = null!;
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    public int Score { get; set; }
    public DateTime RatedOn { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}