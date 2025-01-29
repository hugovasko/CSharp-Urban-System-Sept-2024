namespace UrbanSystem.Web.ViewModels;

public class ProjectRatingViewModel
{
    public Guid ProjectId { get; set; }
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public int? UserRating { get; set; }
    public bool HasUserRated { get; set; }
}

public class CommentRatingViewModel
{
    public Guid CommentId { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public bool? UserRating { get; set; } // true for like, false for dislike, null if not rated
    public bool HasUserRated { get; set; }
}