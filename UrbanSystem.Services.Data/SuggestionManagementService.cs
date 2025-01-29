using static UrbanSystem.Common.ValidationStrings.Suggestion;
using static UrbanSystem.Common.ValidationStrings.Formatting;
using static UrbanSystem.Common.ValidationStrings.Location;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.ViewModels;
using UrbanSystem.Web.ViewModels.Suggestions;

namespace UrbanSystem.Services.Data
{
    public class SuggestionManagementService : ISuggestionManagementService
    {
        private readonly IRepository<Suggestion, Guid> _suggestionRepository;
        private readonly IRepository<Location, Guid> _locationRepository;

        public SuggestionManagementService(
            IRepository<Suggestion, Guid> suggestionRepository,
            IRepository<Location, Guid> locationRepository)
        {
            _suggestionRepository = suggestionRepository;
            _locationRepository = locationRepository;
        }

        public async Task<IEnumerable<SuggestionIndexViewModel>> GetAllSuggestionsAsync()
        {
            var suggestions = await _suggestionRepository.GetAllAsync();
            return suggestions.Select(s => new SuggestionIndexViewModel
            {
                Id = s.Id.ToString(),
                Title = s.Title ?? UnknownUserMessage,
                Category = s.Category ?? UnknownUserMessage,
                OrganizerName = s.UsersSuggestions.FirstOrDefault()?.User.UserName ?? UnknownUserMessage,
                AttachmentUrl = s.AttachmentUrl,
                Description = s.Description ?? DescriptionRequiredMessage,
                UploadedOn = s.UploadedOn.ToString(DateDisplayFormat),
                Status = s.Status,
                Priority = s.Priority,
                LocationNames = s.SuggestionsLocations.Select(sl => sl.Location.CityName ?? UnknownLocation)
            });
        }

        public async Task<SuggestionIndexViewModel?> GetSuggestionByIdAsync(Guid id)
        {
            var suggestion = await _suggestionRepository.GetByIdAsync(id);
            if (suggestion == null)
            {
                return null;
            }

            return new SuggestionIndexViewModel
            {
                Id = suggestion.Id.ToString(),
                Title = suggestion.Title ?? UnknownUserMessage,
                Category = suggestion.Category ?? UnknownUserMessage,
                OrganizerName = suggestion.UsersSuggestions.FirstOrDefault()?.User.UserName ?? UnknownUserMessage,
                AttachmentUrl = suggestion.AttachmentUrl,
                Description = suggestion.Description ?? DescriptionRequiredMessage,
                UploadedOn = suggestion.UploadedOn.ToString(DateDisplayFormat),
                Status = suggestion.Status,
                Priority = suggestion.Priority,
                LocationNames = suggestion.SuggestionsLocations.Select(sl => sl.Location.CityName ?? UnknownLocation),
                Comments = suggestion.Comments.Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content ?? DescriptionRequiredMessage,
                    AddedOn = c.AddedOn,
                    UserName = c.User.UserName ?? UnknownUserMessage
                }).ToList()
            };
        }

        public async Task<bool> DeleteSuggestionAsync(Guid id)
        {
            var success = await _suggestionRepository.DeleteAsync(id);
            if (!success)
            {
                throw new InvalidOperationException(SuggestionNotFoundMessage);
            }
            return true;
        }

        public async Task<bool> UpdateSuggestionStatusAsync(Guid id, string status)
        {
            var suggestion = await _suggestionRepository.GetByIdAsync(id);
            if (suggestion == null)
            {
                throw new ArgumentException(SuggestionNotFoundMessage);
            }

            suggestion.Status = status ?? throw new ArgumentException(StatusRequiredMessage);
            return await _suggestionRepository.UpdateAsync(suggestion);
        }

        public async Task<bool> UpdateSuggestionPriorityAsync(Guid id, string priority)
        {
            var suggestion = await _suggestionRepository.GetByIdAsync(id);
            if (suggestion == null)
            {
                throw new ArgumentException(SuggestionNotFoundMessage);
            }

            suggestion.Priority = priority ?? throw new ArgumentException(PriorityRequiredMessage);
            return await _suggestionRepository.UpdateAsync(suggestion);
        }
    }
}