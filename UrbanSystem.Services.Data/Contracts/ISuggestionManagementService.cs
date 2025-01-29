using UrbanSystem.Web.ViewModels.Suggestions;

namespace UrbanSystem.Services.Data.Contracts
{
    public interface ISuggestionManagementService
    {
        Task<IEnumerable<SuggestionIndexViewModel>> GetAllSuggestionsAsync();
        Task<SuggestionIndexViewModel?> GetSuggestionByIdAsync(Guid id);
        Task<bool> DeleteSuggestionAsync(Guid id);
        Task<bool> UpdateSuggestionStatusAsync(Guid id, string status);
        Task<bool> UpdateSuggestionPriorityAsync(Guid id, string priority);
    }
}