using UrbanSystem.Web.ViewModels.Suggestions;

namespace UrbanSystem.Services.Data.Contracts
{
    public interface IMySuggestionService
    {
        Task<IEnumerable<MySuggestionsViewModel>> GetAllSuggestionsForLoggedInUser(string userId);
    }
}
