using UrbanSystem.Web.ViewModels.Meetings;

namespace UrbanSystem.Services.Data.Contracts
{
    public interface IMeetingService
    {
        Task<IEnumerable<MeetingIndexViewModel>> GetAllMeetingsAsync();
        Task<MeetingIndexViewModel> GetMeetingByIdAsync(Guid id);
        Task<MeetingFormViewModel> GetMeetingFormViewModelAsync(MeetingFormViewModel existingModel = null);
        Task<MeetingEditViewModel> GetMeetingForEditAsync(Guid id);
        Task<Guid> CreateMeetingAsync(MeetingFormViewModel meetingForm, string organizerName);
        Task UpdateMeetingAsync(Guid id, MeetingEditViewModel meetingForm);
        Task DeleteMeetingAsync(Guid id);
        Task AttendMeetingAsync(string username, Guid meetingId);
        Task CancelAttendanceAsync(string username, Guid meetingId);
        Task<UserAttendedMeetingsViewModel> GetUserAttendedMeetingsAsync(string username);
    }
}