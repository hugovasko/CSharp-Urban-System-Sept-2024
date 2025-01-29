namespace UrbanSystem.Services.Data.Contracts;

public interface IRatingService
{
    Task<bool> RateProjectAsync(Guid projectId, Guid userId, int score);
    Task<bool> RateCommentAsync(Guid commentId, Guid userId, bool isLike);
    Task<double> GetProjectAverageRatingAsync(Guid projectId);
    Task<int> GetProjectTotalRatingsAsync(Guid projectId);
    Task<(int Likes, int Dislikes)> GetCommentRatingStatsAsync(Guid commentId);
    Task<bool> HasUserRatedProjectAsync(Guid projectId, Guid userId);
    Task<bool> HasUserRatedCommentAsync(Guid commentId, Guid userId);
    Task<bool> DeleteProjectRatingAsync(Guid projectId, Guid userId);
    Task<bool> DeleteCommentRatingAsync(Guid commentId, Guid userId);
}