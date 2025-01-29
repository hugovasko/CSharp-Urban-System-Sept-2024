// RatingController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrbanSystem.Data.Models;
using UrbanSystem.Services.Data.Contracts;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[AutoValidateAntiforgeryToken]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RatingController(
        IRatingService ratingService,
        UserManager<ApplicationUser> userManager)
    {
        _ratingService = ratingService;
        _userManager = userManager;
    }

    [HttpPost("project/{projectId}")]
    public async Task<IActionResult> RateProject(Guid projectId, [FromBody] int score)
    {
        if (score < 1 || score > 5)
            return BadRequest("Score must be between 1 and 5");

        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var result = await _ratingService.RateProjectAsync(projectId, userId, score);

        if (result)
        {
            var averageRating = await _ratingService.GetProjectAverageRatingAsync(projectId);
            var totalRatings = await _ratingService.GetProjectTotalRatingsAsync(projectId);
            return Ok(new { averageRating });
        }

        return BadRequest("Failed to rate project");
    }

    [HttpPost("comment/{commentId}")]
    public async Task<IActionResult> RateComment(Guid commentId, [FromBody] bool isLike)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var result = await _ratingService.RateCommentAsync(commentId, userId, isLike);

        if (result)
        {
            var stats = await _ratingService.GetCommentRatingStatsAsync(commentId);
            return Ok(new { stats.Likes, stats.Dislikes });
        }

        return BadRequest("Failed to rate comment");
    }
}