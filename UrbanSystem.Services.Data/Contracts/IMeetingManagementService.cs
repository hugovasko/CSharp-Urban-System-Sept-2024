using UrbanSystem.Web.ViewModels.Meetings;
using UrbanSystem.Web.ViewModels.Locations;

namespace UrbanSystem.Services.Data.Contracts
{
    public interface IMeetingManagementService
    {
        Task<IEnumerable<MeetingIndexViewModel>> GetAllMeetingsAsync();
        Task<bool> DeleteMeetingAsync(Guid id);
        Task<bool> CreateMeetingAsync(MeetingFormViewModel model);
        Task<IEnumerable<CityOption>> GetAllCitiesAsync();
    }
}