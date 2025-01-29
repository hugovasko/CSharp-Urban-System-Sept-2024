using Microsoft.EntityFrameworkCore;
using static UrbanSystem.Common.ValidationStrings.Meeting;
using static UrbanSystem.Common.ValidationStrings.Location;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.ViewModels.Locations;
using UrbanSystem.Web.ViewModels.Meetings;

namespace UrbanSystem.Services.Data
{
    public class MeetingService : IMeetingService
    {
        private readonly IRepository<Meeting, Guid> _meetingRepository;
        private readonly IRepository<Location, Guid> _locationRepository;
        private readonly IRepository<ApplicationUser, Guid> _userRepository;

        public MeetingService(
            IRepository<Meeting, Guid> meetingRepository,
            IRepository<Location, Guid> locationRepository,
            IRepository<ApplicationUser, Guid> userRepository)
        {
            _meetingRepository = meetingRepository;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<MeetingIndexViewModel>> GetAllMeetingsAsync()
        {
            var meetings = await _meetingRepository.GetAllAttached()
                .Include(m => m.Location)
                .Include(m => m.Attendees)
                .ToListAsync();

            return meetings.Select(m => new MeetingIndexViewModel
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ScheduledDate = m.ScheduledDate,
                Duration = m.Duration,
                CityName = m.Location?.CityName ?? UnknownLocation,
                AttendeesCount = m.Attendees.Count,
                Latitude = m.Latitude,
                Longitude = m.Longitude
            });
        }

        public async Task<MeetingIndexViewModel> GetMeetingByIdAsync(Guid id)
        {
            var meeting = await _meetingRepository.GetAllAttached()
                .Include(m => m.Location)
                .Include(m => m.Attendees)
                .Include(m => m.Organizer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meeting == null)
            {
                return null!;
            }

            return new MeetingIndexViewModel
            {
                Id = meeting.Id,
                Title = meeting.Title,
                Description = meeting.Description,
                ScheduledDate = meeting.ScheduledDate,
                Duration = meeting.Duration,
                CityName = meeting.Location?.CityName ?? UnknownLocation,
                Attendees = meeting.Attendees.Select(a => a.UserName).ToList()!,
                OrganizerName = meeting.Organizer.UserName!,
                OrganizerId = meeting.OrganizerId,
                Latitude = meeting.Latitude,
                Longitude = meeting.Longitude
            };
        }

        public async Task<MeetingFormViewModel> GetMeetingFormViewModelAsync(MeetingFormViewModel existingModel = null!)
        {
            var locations = await _locationRepository.GetAllAsync();
            var cities = locations.Select(l => new CityOption
            {
                Value = l.Id.ToString(),
                Text = l.CityName
            }).AsEnumerable();

            return new MeetingFormViewModel
            {
                Title = existingModel?.Title ?? string.Empty,
                Description = existingModel?.Description ?? string.Empty,
                ScheduledDate = existingModel?.ScheduledDate ?? DateTime.Now,
                Duration = existingModel?.Duration ?? 1,
                LocationId = existingModel?.LocationId,
                Cities = cities
            };
        }

        public async Task<MeetingEditViewModel> GetMeetingEditViewModelAsync(MeetingFormViewModel existingModel = null!)
        {
            var locations = await _locationRepository.GetAllAsync();
            var cities = locations.Select(l => new CityOption
            {
                Value = l.Id.ToString(),
                Text = l.CityName
            }).AsEnumerable();

            return new MeetingEditViewModel
            {
                Title = existingModel?.Title ?? string.Empty,
                Description = existingModel?.Description ?? string.Empty,
                ScheduledDate = existingModel?.ScheduledDate ?? DateTime.Now,
                Duration = existingModel?.Duration ?? 1,
                LocationId = existingModel?.LocationId.ToString(),
                Cities = cities
            };
        }

        public async Task<MeetingEditViewModel> GetMeetingForEditAsync(Guid id)
        {
            var meeting = await _meetingRepository.GetByIdAsync(id);
            if (meeting == null)
            {
                return null!;
            }

            var viewModel = await GetMeetingEditViewModelAsync();
            viewModel.Title = meeting.Title;
            viewModel.Description = meeting.Description;
            viewModel.ScheduledDate = meeting.ScheduledDate;
            viewModel.Duration = 0.0;
            viewModel.LocationId = meeting.LocationId.ToString();
            viewModel.Latitude = meeting.Latitude;
            viewModel.Longitude = meeting.Longitude;

            return viewModel;
        }

        public async Task<Guid> CreateMeetingAsync(MeetingFormViewModel meetingForm, string organizerUsername)
        {
            var location = await _locationRepository.GetByIdAsync(meetingForm.LocationId ?? Guid.Empty);
            if (location == null)
            {
                throw new ArgumentException(InvalidLocationId);
            }

            var organizer = await _userRepository.GetAllAttached().FirstOrDefaultAsync(u => u.UserName == organizerUsername);
            if (organizer == null)
            {
                throw new ArgumentException(OrganizerNotFound);
            }

            var meeting = new Meeting
            {
                Title = meetingForm.Title,
                Description = meetingForm.Description,
                ScheduledDate = meetingForm.ScheduledDate,
                Duration = meetingForm.Duration,
                LocationId = location.Id,
                Location = location,
                OrganizerId = organizer.Id,
                Organizer = organizer,
                Latitude = meetingForm.Latitude,
                Longitude = meetingForm.Longitude
            };

            await _meetingRepository.AddAsync(meeting);
            return meeting.Id;
        }

        public async Task UpdateMeetingAsync(Guid id, MeetingEditViewModel meetingForm)
        {
            var meeting = await _meetingRepository.GetAllAttached()
                .Include(m => m.Location)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meeting == null)
            {
                throw new ArgumentException(MeetingNotFound, nameof(id));
            }

            var location = await _locationRepository.GetByIdAsync(Guid.Parse(meetingForm.LocationId!));
            if (location == null)
            {
                throw new ArgumentException(InvalidLocationId);
            }

            meeting.Title = meetingForm.Title;
            meeting.Description = meetingForm.Description;
            meeting.ScheduledDate = meetingForm.ScheduledDate;
            meeting.Duration = meetingForm.Duration;
            meeting.LocationId = location.Id;
            meeting.Location = location;
            meeting.Latitude = meetingForm.Latitude;
            meeting.Longitude = meetingForm.Longitude;

            await _meetingRepository.UpdateAsync(meeting);
        }

        public async Task DeleteMeetingAsync(Guid id)
        {
            var success = await _meetingRepository.DeleteAsync(id);
            if (!success)
            {
                throw new ArgumentException(MeetingNotFound, nameof(id));
            }
        }

        public async Task AttendMeetingAsync(string username, Guid meetingId)
        {
            var meeting = await _meetingRepository.GetAllAttached()
                .Include(m => m.Attendees)
                .FirstOrDefaultAsync(m => m.Id == meetingId);

            if (meeting == null)
            {
                throw new InvalidOperationException(MeetingNotFound);
            }

            var user = await _userRepository.GetAllAttached().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new InvalidOperationException(UserNotFound);
            }

            if (meeting.Attendees.Any(a => a.Id == user.Id))
            {
                throw new InvalidOperationException(AlreadyAttendingMeeting);
            }

            meeting.Attendees.Add(user);
            await _meetingRepository.UpdateAsync(meeting);
        }

        public async Task CancelAttendanceAsync(string username, Guid meetingId)
        {
            var meeting = await _meetingRepository.GetAllAttached()
                .Include(m => m.Attendees)
                .FirstOrDefaultAsync(m => m.Id == meetingId);

            if (meeting == null)
            {
                throw new InvalidOperationException(MeetingNotFound);
            }

            var user = await _userRepository.GetAllAttached().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new InvalidOperationException(UserNotFound);
            }

            var attendee = meeting.Attendees.FirstOrDefault(a => a.Id == user.Id);
            if (attendee == null)
            {
                throw new InvalidOperationException(NotAttendingMeeting);
            }

            meeting.Attendees.Remove(attendee);
            await _meetingRepository.UpdateAsync(meeting);
        }

        public async Task<UserAttendedMeetingsViewModel> GetUserAttendedMeetingsAsync(string username)
        {
            var user = await _userRepository.GetAllAttached()
                .Include(u => u.Meetings)
                .ThenInclude(m => m.Location)
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                return new UserAttendedMeetingsViewModel { AttendedMeetings = new List<AttendedMeetingViewModel>() };
            }

            var attendedMeetings = user.Meetings.Select(m => new AttendedMeetingViewModel
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ScheduledDate = m.ScheduledDate,
                Duration = m.Duration,
                Location = m.Location?.CityName ?? UnknownLocation,
                CanCancelAttendance = m.ScheduledDate > DateTime.Now.AddHours(24)
            }).ToList();

            return new UserAttendedMeetingsViewModel { AttendedMeetings = attendedMeetings };
        }
    }
}