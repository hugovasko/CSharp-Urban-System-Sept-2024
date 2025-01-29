using UrbanSystem.Web.Helpers;
using UrbanSystem.Web.ViewModels.Locations;

namespace UrbanSystem.Services.Data.Contracts
{
    public interface ILocationService
    {
        Task AddLocationAsync(LocationFormViewModel model);
        Task<PaginatedList<LocationDetailsViewModel>> GetAllOrderedByNameAsync(int pageIndex, int pageSize);
        Task<LocationDetailsViewModel> GetLocationDetailsByIdAsync(string? id);
    }
}