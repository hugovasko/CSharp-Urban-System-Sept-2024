using UrbanSystem.Data.Models;
using UrbanSystem.Web.ViewModels;
using UrbanSystem.Web.ViewModels.Suggestions;
using UrbanSystem.Web.Helpers;

namespace UrbanSystem.Services.Data.Contracts
{
    public interface ISuggestionService
    {
        Task<PaginatedList<SuggestionIndexViewModel>> GetAllSuggestionsAsync(int pageIndex, int pageSize, string searchQuery = "", string sortBy = "", bool ascending = true);
        Task<(bool IsSuccessful, SuggestionFormViewModel ViewModel, string ErrorMessage)> AddSuggestionAsync(SuggestionFormViewModel suggestionModel, string userId);
        Task<(bool IsSuccessful, SuggestionIndexViewModel Suggestion, string ErrorMessage)> GetSuggestionDetailsAsync(string id, string userId);
        Task<(bool IsSuccessful, string ErrorMessage)> AddCommentAsync(string suggestionId, string content, string userId);
        List<SuggestionIndexViewModel> SortSuggestions(List<SuggestionIndexViewModel> suggestions, string sortBy, bool ascending);
        Task<CommentViewModel?> GetCommentAsync(Guid commentId);
        Task<(bool IsSuccessful, string ErrorMessage)> UpdateSuggestionAsync(string id, SuggestionFormViewModel model, string userId);
        Task<(bool IsSuccessful, SuggestionFormViewModel ViewModel, string ErrorMessage)> GetSuggestionForEditAsync(string id, ApplicationUser user);
        Task<(bool IsSuccessful, string ErrorMessage)> DeleteSuggestionAsync(string id, string userId);
        Task<(bool IsSuccessful, ConfirmDeleteViewModel ViewModel, string ErrorMessage)> GetSuggestionForDeleteConfirmationAsync(string id, string userId);
        Task<SuggestionFormViewModel> GetSuggestionFormViewModelAsync();
    }
}