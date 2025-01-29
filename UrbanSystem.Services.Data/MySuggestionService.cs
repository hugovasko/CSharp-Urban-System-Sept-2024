using Microsoft.EntityFrameworkCore;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.ViewModels.Locations;
using UrbanSystem.Web.ViewModels.Suggestions;
using static UrbanSystem.Common.ValidationStrings.Formatting;

namespace UrbanSystem.Services.Data
{
    public class MySuggestionService : IMySuggestionService
    {
        private readonly IRepository<ApplicationUserSuggestion, object> _userSuggestionRepository;

        public MySuggestionService(IRepository<ApplicationUserSuggestion, object> userSuggestionRepository)
        {
            _userSuggestionRepository = userSuggestionRepository;
        }

        public async Task<IEnumerable<MySuggestionsViewModel>> GetAllSuggestionsForLoggedInUser(string userId)
        {
            var suggestions = await _userSuggestionRepository
                .GetAllAttached()
                .Include(us => us.Suggestion)
                .ThenInclude(s => s.SuggestionsLocations)
                    .ThenInclude(sl => sl.Location)
                .Where(us => us.ApplicationUserId.ToString().ToLower() == userId.ToLower())
                .OrderBy(us => us.Suggestion.UploadedOn)
                .ToListAsync();

            var viewModel = suggestions.Select(us => new MySuggestionsViewModel
            {
                Id = us.Suggestion.Id.ToString(),
                Title = us.Suggestion.Title,
                Category = us.Suggestion.Category,
                UploadedOn = us.Suggestion.UploadedOn.ToString(SuggestionUploadedOnFormat),
                AttachmentUrl = us.Suggestion.AttachmentUrl,
                LocationNames = us.Suggestion.SuggestionsLocations?
                    .Where(sl => sl.Location != null)
                    .Select(sl => new CityOption
                    {
                        Value = sl.Location.Id.ToString(),
                        Text = sl.Location.CityName
                    }).ToList() ?? new List<CityOption>()
            });

            return viewModel;
        }
    }
}
