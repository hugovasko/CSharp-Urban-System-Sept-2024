using Microsoft.EntityFrameworkCore;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.ViewModels.Meetings;
using UrbanSystem.Web.ViewModels.Locations;

namespace UrbanSystem.Services.Data
{
    public class MeetingManagementService : IMeetingManagementService
    {
        private readonly IRepository<Meeting, Guid> _meetingRepository;
        private readonly IRepository<Location, Guid> _locationRepository;
        private readonly IRepository<ApplicationUser, string> _userRepository;

        public MeetingManagementService(
            IRepository<Meeting, Guid> meetingRepository,
            IRepository<Location, Guid> locationRepository,
            IRepository<ApplicationUser, string> userRepository)
        {
            _meetingRepository = meetingRepository;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<MeetingIndexViewModel>> GetAllMeetingsAsync()
        {
            var meetings = await _meetingRepository.GetAllAttached()
                .Include(m => m.Location)
                .Include(m => m.Organizer)
                .Include(m => m.Attendees)
                .ToListAsync();

            return meetings.Select(m => new MeetingIndexViewModel
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ScheduledDate = m.ScheduledDate,
                Duration = m.Duration,
                LocationId = m.LocationId,
                CityName = m.Location.CityName,
                AttendeesCount = m.Attendees.Count,
                Attendees = m.Attendees.Select(a => a.UserName)!,
                OrganizerName = m.Organizer.UserName!,
                OrganizerId = m.OrganizerId,
                IsCurrentUserOrganizer = false
            });
        }

        public async Task<bool> DeleteMeetingAsync(Guid id)
        {
            return await _meetingRepository.DeleteAsync(id);
        }

        public async Task<bool> CreateMeetingAsync(MeetingFormViewModel model)
        {
            var meeting = new Meeting
            {
                Title = model.Title,
                Description = model.Description,
                ScheduledDate = model.ScheduledDate,
                Duration = model.Duration,
                LocationId = model.LocationId.Value
            };

            await _meetingRepository.AddAsync(meeting);
            return true;
        }

        public async Task<IEnumerable<CityOption>> GetAllCitiesAsync()
        {
            var cities = await _locationRepository.GetAllAsync();
            return cities.Select(c => new CityOption
            {
                Value = c.Id.ToString(),
                Text = c.CityName
            });
        }
    }
}