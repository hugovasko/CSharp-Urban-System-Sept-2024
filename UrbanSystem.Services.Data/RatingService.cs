using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data.Contracts;

namespace UrbanSystem.Services.Data
{
    public class RatingService : IRatingService
    {
        private readonly IRepository<ProjectRating, Guid> _projectRatingRepository;
        private readonly IRepository<CommentRating, Guid> _commentRatingRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<Comment, Guid> _commentRepository;

        public RatingService(
            IRepository<ProjectRating, Guid> projectRatingRepository,
            IRepository<CommentRating, Guid> commentRatingRepository,
            IRepository<Project, Guid> projectRepository,
            IRepository<Comment, Guid> commentRepository)
        {
            _projectRatingRepository = projectRatingRepository;
            _commentRatingRepository = commentRatingRepository;
            _projectRepository = projectRepository;
            _commentRepository = commentRepository;
        }

        public async Task<bool> RateProjectAsync(Guid projectId, Guid userId, int score)
        {
            if (score < 1 || score > 5)
                return false;

            var existingRating = (await _projectRatingRepository
                .GetAllAsync(r => r.ProjectId == projectId && r.UserId == userId))
                .FirstOrDefault();

            if (existingRating != null)
            {
                existingRating.Score = score;
                existingRating.RatedOn = DateTime.UtcNow;
                return await _projectRatingRepository.UpdateAsync(existingRating);
            }

            var rating = new ProjectRating
            {
                ProjectId = projectId,
                UserId = userId,
                Score = score
            };

            await _projectRatingRepository.AddAsync(rating);
            return true;
        }

        public async Task<bool> RateCommentAsync(Guid commentId, Guid userId, bool isLike)
        {
            var existingRating = (await _commentRatingRepository
                .GetAllAsync(r => r.CommentId == commentId && r.UserId == userId))
                .FirstOrDefault();

            if (existingRating != null)
            {
                existingRating.IsLike = isLike;
                existingRating.RatedOn = DateTime.UtcNow;
                return await _commentRatingRepository.UpdateAsync(existingRating);
            }

            var rating = new CommentRating
            {
                CommentId = commentId,
                UserId = userId,
                IsLike = isLike
            };

            await _commentRatingRepository.AddAsync(rating);
            return true;
        }

        public async Task<double> GetProjectAverageRatingAsync(Guid projectId)
        {
            var ratings = await _projectRatingRepository
                .GetAllAsync(r => r.ProjectId == projectId && !r.IsDeleted);

            if (!ratings.Any())
                return 0;

            return ratings.Average(r => r.Score);
        }
        
        public async Task<int> GetProjectTotalRatingsAsync(Guid projectId)
        {
            return await _projectRatingRepository.CountAsync(r => r.ProjectId == projectId && !r.IsDeleted);
        }

        public async Task<(int Likes, int Dislikes)> GetCommentRatingStatsAsync(Guid commentId)
        {
            var ratings = await _commentRatingRepository
                .GetAllAsync(r => r.CommentId == commentId && !r.IsDeleted);

            int likes = ratings.Count(r => r.IsLike);
            int dislikes = ratings.Count(r => !r.IsLike);

            return (likes, dislikes);
        }

        public async Task<bool> HasUserRatedProjectAsync(Guid projectId, Guid userId)
        {
            var rating = (await _projectRatingRepository
                .GetAllAsync(r => r.ProjectId == projectId && r.UserId == userId && !r.IsDeleted))
                .FirstOrDefault();

            return rating != null;
        }
        
        public async Task<int?> GetUserProjectRatingAsync(Guid projectId, Guid userId)
        {
            var rating = (await _projectRatingRepository
                    .GetAllAsync(r => r.ProjectId == projectId && r.UserId == userId && !r.IsDeleted))
                .FirstOrDefault();

            return rating?.Score;
        }

        public async Task<bool> HasUserRatedCommentAsync(Guid commentId, Guid userId)
        {
            var rating = (await _commentRatingRepository
                .GetAllAsync(r => r.CommentId == commentId && r.UserId == userId && !r.IsDeleted))
                .FirstOrDefault();

            return rating != null;
        }

        public async Task<bool> DeleteProjectRatingAsync(Guid projectId, Guid userId)
        {
            var rating = (await _projectRatingRepository
                .GetAllAsync(r => r.ProjectId == projectId && r.UserId == userId))
                .FirstOrDefault();

            if (rating == null)
                return false;

            rating.IsDeleted = true;
            return await _projectRatingRepository.UpdateAsync(rating);
        }

        public async Task<bool> DeleteCommentRatingAsync(Guid commentId, Guid userId)
        {
            var rating = (await _commentRatingRepository
                .GetAllAsync(r => r.CommentId == commentId && r.UserId == userId))
                .FirstOrDefault();

            if (rating == null)
                return false;

            rating.IsDeleted = true;
            return await _commentRatingRepository.UpdateAsync(rating);
        }
    }
}