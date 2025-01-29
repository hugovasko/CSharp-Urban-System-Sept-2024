using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.ViewModels;
using UrbanSystem.Web.ViewModels.Locations;
using UrbanSystem.Web.ViewModels.Suggestions;
using static UrbanSystem.Common.ValidationStrings.Suggestion;
using static UrbanSystem.Common.ValidationStrings.Sorting;
using static UrbanSystem.Common.ValidationStrings.Formatting;
using UrbanSystem.Web.Helpers;

namespace UrbanSystem.Services.Data
{
    public class SuggestionService : ISuggestionService
    {
        private readonly IRepository<Suggestion, Guid> _suggestionRepository;
        private readonly IRepository<Location, Guid> _locationRepository;
        private readonly IRepository<ApplicationUserSuggestion, object> _userSuggestionRepository;
        private readonly IRepository<SuggestionLocation, object> _suggestionLocationRepository;
        private readonly IRepository<Comment, Guid> _commentRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public SuggestionService(
            IRepository<Suggestion, Guid> suggestionRepository,
            IRepository<Location, Guid> locationRepository,
            IRepository<ApplicationUserSuggestion, object> userSuggestionRepository,
            IRepository<SuggestionLocation, object> suggestionLocationRepository,
            IRepository<Comment, Guid> commentRepository,
            UserManager<ApplicationUser> userManager)
        {
            _suggestionRepository = suggestionRepository;
            _locationRepository = locationRepository;
            _userSuggestionRepository = userSuggestionRepository;
            _suggestionLocationRepository = suggestionLocationRepository;
            _commentRepository = commentRepository;
            _userManager = userManager;
        }

        public async Task<PaginatedList<SuggestionIndexViewModel>> GetAllSuggestionsAsync(int pageIndex, int pageSize, string searchQuery = "", string sortBy = "", bool ascending = true)
        {
            var suggestionsQuery = _suggestionRepository.GetAllAttached();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                suggestionsQuery = suggestionsQuery.Where(s => s.Title.Contains(searchQuery) || s.Description.Contains(searchQuery));
            }

            var totalCount = await suggestionsQuery.CountAsync();

            var suggestions = await suggestionsQuery
                .Select(suggestion => new SuggestionIndexViewModel
                {
                    Id = suggestion.Id.ToString(),
                    Title = suggestion.Title,
                    Category = suggestion.Category,
                    Description = suggestion.Description,
                    AttachmentUrl = suggestion.AttachmentUrl,
                    UploadedOn = suggestion.UploadedOn.ToString(DateDisplayFormat),
                    Status = suggestion.Status,
                    Priority = suggestion.Priority,
                })
                .ToListAsync();

            suggestions = SortSuggestions(suggestions, sortBy, ascending);

            var paginatedSuggestions = suggestions
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<SuggestionIndexViewModel>(paginatedSuggestions, totalCount, pageIndex, pageSize);
        }

        public async Task<SuggestionFormViewModel> GetSuggestionFormViewModelAsync()
        {
            var cities = await _locationRepository.GetAllAsync();
            return new SuggestionFormViewModel
            {
                Cities = cities.Select(c => new CityOption { Value = c.Id.ToString(), Text = c.CityName }).ToList()
            };
        }

        public List<SuggestionIndexViewModel> SortSuggestions(List<SuggestionIndexViewModel> suggestions, string sortBy, bool ascending)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return ascending
                    ? suggestions.OrderBy(s => DateTime.TryParse(s.UploadedOn, out var date) ? date : DateTime.MinValue).ToList()
                    : suggestions.OrderByDescending(s => DateTime.TryParse(s.UploadedOn, out var date) ? date : DateTime.MinValue).ToList();
            }

            switch (sortBy.ToLower())
            {
                case SortByTitleMessage:
                    return ascending ? suggestions.OrderBy(s => s.Title).ToList() : suggestions.OrderByDescending(s => s.Title).ToList();
                case SortByDateMessage:
                    return ascending
                        ? suggestions.OrderBy(s => DateTime.TryParse(s.UploadedOn, out var date) ? date : DateTime.MinValue).ToList()
                        : suggestions.OrderByDescending(s => DateTime.TryParse(s.UploadedOn, out var date) ? date : DateTime.MinValue).ToList();
                default:
                    return suggestions;
            }
        }

        public async Task<(bool IsSuccessful, SuggestionFormViewModel ViewModel, string ErrorMessage)> AddSuggestionAsync(SuggestionFormViewModel suggestionModel, string userId)
        {
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid parsedUserId))
            {
                return (false, suggestionModel, InvalidUserIdMessage);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, suggestionModel, UserNotFoundMessage);
            }

            var location = await _locationRepository.GetByIdAsync(suggestionModel.CityId);
            if (location == null)
            {
                return (false, suggestionModel, InvalidCitySelectedMessage);
            }

            var suggestion = new Suggestion
            {
                Title = suggestionModel.Title,
                Category = suggestionModel.Category,
                Description = suggestionModel.Description,
                AttachmentUrl = suggestionModel.AttachmentUrl,
                Status = suggestionModel.Status,
                Priority = suggestionModel.Priority,
                UploadedOn = DateTime.UtcNow,
                Longitude = suggestionModel.Longitude,
                Latitude = suggestionModel.Latitude,
            };

            await _suggestionRepository.AddAsync(suggestion);

            var applicationUserSuggestion = new ApplicationUserSuggestion
            {
                ApplicationUserId = user.Id,
                SuggestionId = suggestion.Id
            };

            await _userSuggestionRepository.AddAsync(applicationUserSuggestion);

            var suggestionLocation = new SuggestionLocation
            {
                SuggestionId = suggestion.Id,
                LocationId = location.Id
            };

            await _suggestionLocationRepository.AddAsync(suggestionLocation);

            return (true, null!, null!);
        }

        public async Task<(bool IsSuccessful, SuggestionIndexViewModel Suggestion, string ErrorMessage)> GetSuggestionDetailsAsync(string id, string userId)
        {
            if (!Guid.TryParse(id, out Guid suggestionId))
            {
                return (false, null!, InvalidSuggestionIdMessage);
            }

            var suggestion = await _suggestionRepository
                .GetAllAttached()
                .Include(s => s.Comments)
                .ThenInclude(c => c.User)
                .Include(s => s.UsersSuggestions)
                .ThenInclude(us => us.User)
                .Include(s => s.SuggestionsLocations)
                .ThenInclude(s => s.Location)
                .FirstOrDefaultAsync(s => s.Id == suggestionId);

            if (suggestion == null)
            {
                return (false, null!, SuggestionNotFoundMessage);
            }

            bool isOwner = false;

            if (!string.IsNullOrEmpty(userId))
            {
                if (Guid.TryParse(userId, out var parsedUserId))
                {
                    isOwner = suggestion.UsersSuggestions.Any(us => us.ApplicationUserId == parsedUserId);
                }
            }

            var viewModel = new SuggestionIndexViewModel
            {
                Id = suggestion.Id.ToString(),
                Title = suggestion.Title,
                Category = suggestion.Category,
                Description = suggestion.Description,
                AttachmentUrl = suggestion.AttachmentUrl,
                UploadedOn = suggestion.UploadedOn.ToString(DateDisplayFormat),
                Status = suggestion.Status,
                Priority = suggestion.Priority,
                Longitude = suggestion.Longitude,
                Latitude = suggestion.Latitude,
                LocationNames = suggestion.SuggestionsLocations.Select(sl => sl.Location.CityName).ToList(),
                Comments = suggestion.Comments.Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content ?? string.Empty,
                    AddedOn = c.AddedOn,
                    UserName = c.User?.UserName ?? UnknownUserMessage,
                }).ToList(),
                OrganizerName = string.Join(", ", suggestion.UsersSuggestions.Select(x => x.User.UserName))
            };

            return (true, viewModel, null!);
        }

        public async Task<(bool IsSuccessful, string ErrorMessage)> AddCommentAsync(string suggestionId, string content, string userId)
        {
            if (!Guid.TryParse(suggestionId, out Guid parsedSuggestionId))
            {
                return (false, InvalidSuggestionIdMessage);
            }

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid parsedUserId))
            {
                return (false, InvalidUserIdMessage);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, UserNotFoundMessage);
            }

            var comment = new Comment
            {
                Content = content,
                UserId = parsedUserId,
                SuggestionId = parsedSuggestionId
            };

            await _commentRepository.AddAsync(comment);
            return (true, null!);
        }

        public async Task<CommentViewModel?> GetCommentAsync(Guid commentId)
        {
            if (commentId == Guid.Empty)
            {
                return null;
            }

            var comment = await _commentRepository.GetAllAttached()
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return null;
            }

            return new CommentViewModel
            {
                Id = comment.Id,
                Content = comment.Content ?? string.Empty,
                AddedOn = comment.AddedOn,
                UserName = comment.User?.UserName ?? UnknownUserMessage,
            };
        }

        public async Task<(bool IsSuccessful, SuggestionFormViewModel ViewModel, string ErrorMessage)> GetSuggestionForEditAsync(string id, ApplicationUser user)
        {
            if (!Guid.TryParse(id, out Guid suggestionId))
            {
                return (false, null!, InvalidSuggestionIdMessage);
            }

            var suggestion = await _suggestionRepository
            .GetAllAttached()
            .Include(s => s.SuggestionsLocations)
            .ThenInclude(sl => sl.Location)
            .Include(s => s.UsersSuggestions)
            .FirstOrDefaultAsync(s => s.Id == suggestionId);

            if (suggestion == null)
            {
                return (false, null!, SuggestionNotFoundMessage);
            }

            var isOriginalPoster = suggestion.UsersSuggestions.Any(us => us.ApplicationUserId == user.Id);
            if (!isOriginalPoster)
            {
                return (false, null!, NotAuthorizedToEditMessage);
            }

            var cities = await _locationRepository.GetAllAsync();

            var viewModel = new SuggestionFormViewModel
            {
                Id = suggestion.Id,
                Title = suggestion.Title,
                Category = suggestion.Category,
                Description = suggestion.Description,
                AttachmentUrl = suggestion.AttachmentUrl,
                Status = suggestion.Status,
                Priority = suggestion.Priority,
                Longitude = suggestion.Longitude,
                Latitude = suggestion.Latitude,
                CityId = suggestion.SuggestionsLocations.FirstOrDefault()?.LocationId ?? Guid.Empty,
                Cities = cities.Select(c => new CityOption { Value = c.Id.ToString(), Text = c.CityName }).ToList(),
                UserId = user.Id.ToString()
            };

            return (true, viewModel, null!);
        }

        public async Task<(bool IsSuccessful, string ErrorMessage)> UpdateSuggestionAsync(string id, SuggestionFormViewModel model, string userId)
        {
            if (!Guid.TryParse(id, out Guid suggestionId))
            {
                return (false, InvalidSuggestionIdMessage);
            }

            var suggestion = await _suggestionRepository
                .GetAllAttached()
                .Include(s => s.SuggestionsLocations)
                .Include(s => s.UsersSuggestions)
                .FirstOrDefaultAsync(s => s.Id == suggestionId);

            if (suggestion == null)
            {
                return (false, SuggestionNotFoundMessage);
            }

            var userSuggestion = suggestion.UsersSuggestions.FirstOrDefault();
            if (userSuggestion == null || userSuggestion.ApplicationUserId.ToString() != userId)
            {
                return (false, NotAuthorizedToEditMessage);
            }

            suggestion.Title = model.Title;
            suggestion.Category = model.Category;
            suggestion.Description = model.Description;
            suggestion.AttachmentUrl = model.AttachmentUrl;
            suggestion.Status = model.Status;
            suggestion.Priority = model.Priority;
            suggestion.Latitude = model.Latitude;
            suggestion.Longitude = model.Longitude;

            var existingLocation = suggestion.SuggestionsLocations.FirstOrDefault();
            if (existingLocation != null && existingLocation.LocationId != model.CityId)
            {
                await _suggestionLocationRepository.DeleteAsync(sl => sl.SuggestionId == suggestion.Id && sl.LocationId == existingLocation.LocationId);

                var newLocation = new SuggestionLocation
                {
                    SuggestionId = suggestion.Id,
                    LocationId = model.CityId
                };
                await _suggestionLocationRepository.AddAsync(newLocation);
            }

            await _suggestionRepository.UpdateAsync(suggestion);

            return (true, null!);
        }

        public async Task<(bool IsSuccessful, ConfirmDeleteViewModel ViewModel, string ErrorMessage)> GetSuggestionForDeleteConfirmationAsync(string id, string userId)
        {
            if (!Guid.TryParse(id, out Guid suggestionId))
            {
                return (false, null!, InvalidSuggestionIdMessage);
            }

            var suggestion = await _suggestionRepository
                .GetAllAttached()
                .Include(s => s.UsersSuggestions)
                .FirstOrDefaultAsync(s => s.Id == suggestionId);

            if (suggestion == null)
            {
                return (false, null!, SuggestionNotFoundMessage);
            }

            var userSuggestion = suggestion.UsersSuggestions.FirstOrDefault();
            if (userSuggestion == null || userSuggestion.ApplicationUserId.ToString() != userId)
            {
                return (false, null!, NotAuthorizedToDeleteMessage);
            }

            var viewModel = new ConfirmDeleteViewModel
            {
                Id = suggestion.Id,
                Title = suggestion.Title,
                Category = suggestion.Category,
                Description = suggestion.Description
            };

            return (true, viewModel, null!);
        }

        public async Task<(bool IsSuccessful, string ErrorMessage)> DeleteSuggestionAsync(string id, string userId)
        {
            if (!Guid.TryParse(id, out Guid suggestionId))
            {
                return (false, InvalidSuggestionIdMessage);
            }

            var suggestion = await _suggestionRepository
                .GetAllAttached()
                .Include(s => s.UsersSuggestions)
                .FirstOrDefaultAsync(s => s.Id == suggestionId);

            if (suggestion == null)
            {
                return (false, SuggestionNotFoundMessage);
            }

            var userSuggestion = suggestion.UsersSuggestions.FirstOrDefault();
            if (userSuggestion == null || userSuggestion.ApplicationUserId.ToString() != userId)
            {
                return (false, NotAuthorizedToDeleteMessage);
            }

            await _suggestionLocationRepository.DeleteAsync(sl => sl.SuggestionId == suggestion.Id);
            await _userSuggestionRepository.DeleteAsync(us => us.SuggestionId == suggestion.Id);

            var comments = await _commentRepository.GetAllAttached()
                .Where(c => c.SuggestionId == suggestion.Id)
                .ToListAsync();

            foreach (var comment in comments)
            {
                await _commentRepository.DeleteAsync(comment.Id);
            }

            await _suggestionRepository.DeleteAsync(suggestion.Id);

            return (true, null!);
        }
    }
}