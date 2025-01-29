using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrbanSystem.Data.Models;
using UrbanSystem.Web.ViewModels.Locations;

namespace UrbanSystem.Services.Data.Contracts
{
    public interface ILocationManagementService
    {
        Task<IEnumerable<LocationDetailsViewModel>> GetAllLocationsAsync();
        Task<Location> GetLocationByIdAsync(Guid locationId);
        Task<bool> DeleteLocationAsync(Guid locationId);
        Task<bool> LocationExistsByIdAsync(Guid locationId);
        Task DeleteSuggestionsByLocationIdAsync(Guid locationId);
        Task DeleteMeetingsByLocationIdAsync(Guid locationId);
        Task DeleteProjectsByLocationIdAsync(Guid locationId);
    }
}
