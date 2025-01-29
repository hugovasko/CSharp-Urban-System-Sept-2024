using MockQueryable.Moq;
using Moq;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data;
using UrbanSystem.Web.ViewModels.Meetings;

namespace ServiceTests
{
    [TestFixture]
    public class MeetingServiceTests
    {
        private Mock<IRepository<Meeting, Guid>> _mockMeetingRepository;
        private Mock<IRepository<Location, Guid>> _mockLocationRepository;
        private Mock<IRepository<ApplicationUser, Guid>> _mockUserRepository;
        private MeetingService _meetingService;

        [SetUp]
        public void Setup()
        {
            _mockMeetingRepository = new Mock<IRepository<Meeting, Guid>>();
            _mockLocationRepository = new Mock<IRepository<Location, Guid>>();
            _mockUserRepository = new Mock<IRepository<ApplicationUser, Guid>>();

            _meetingService = new MeetingService(
                _mockMeetingRepository.Object,
                _mockLocationRepository.Object,
                _mockUserRepository.Object);
        }

        [Test]
        public async Task GetAllMeetingsAsync_ReturnsAllMeetings_WithCorrectDetails()
        {
            // Arrange
            var testMeetings = new List<Meeting>
            {
                new Meeting
                {
                    Id = Guid.NewGuid(),
                    Title = "Meeting 1",
                    Description = "Description 1",
                    ScheduledDate = DateTime.UtcNow,
                    Duration = 1.0,
                    Location = new Location { CityName = "New York" },
                    Attendees = new List<ApplicationUser> { new ApplicationUser() }
                },
                new Meeting
                {
                    Id = Guid.NewGuid(),
                    Title = "Meeting 2",
                    Description = "Description 2",
                    ScheduledDate = DateTime.UtcNow.AddHours(1),
                    Duration = 2.0,
                    Location = new Location { CityName = "Chicago" },
                    Attendees = new List<ApplicationUser> { new ApplicationUser(), new ApplicationUser() }
                }
            };

            _mockMeetingRepository.Setup(repo => repo.GetAllAttached())
                .Returns(testMeetings.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _meetingService.GetAllMeetingsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Title, Is.EqualTo("Meeting 1"));
            Assert.That(result.First().CityName, Is.EqualTo("New York"));
            Assert.That(result.First().AttendeesCount, Is.EqualTo(1));
        }

        [Test]
        public async Task GetMeetingByIdAsync_ReturnsCorrectMeeting_WhenMeetingExists()
        {
            // Arrange
            var meetingId = Guid.NewGuid();
            var testMeeting = new Meeting
            {
                Id = meetingId,
                Title = "Test Meeting",
                Description = "Test Description",
                ScheduledDate = DateTime.UtcNow,
                Duration = 2.0,
                Location = new Location { CityName = "New York" },
                Organizer = new ApplicationUser { UserName = "Organizer" },
                Attendees = new List<ApplicationUser>
                {
                    new ApplicationUser { UserName = "Attendee1" },
                    new ApplicationUser { UserName = "Attendee2" }
                }
            };

            _mockMeetingRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<Meeting> { testMeeting }.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _meetingService.GetMeetingByIdAsync(meetingId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(meetingId));
            Assert.That(result.Title, Is.EqualTo("Test Meeting"));
            Assert.That(result.OrganizerName, Is.EqualTo("Organizer"));
            Assert.That(result.Attendees.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetMeetingByIdAsync_ReturnsNull_WhenMeetingDoesNotExist()
        {
            // Arrange
            var nonExistentMeetingId = Guid.NewGuid();
            _mockMeetingRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<Meeting>().AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _meetingService.GetMeetingByIdAsync(nonExistentMeetingId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetMeetingFormViewModelAsync_ReturnsCorrectViewModel()
        {
            // Arrange
            var locations = new List<Location>
            {
                new Location { Id = Guid.NewGuid(), CityName = "New York" },
                new Location { Id = Guid.NewGuid(), CityName = "Los Angeles" }
            };
            _mockLocationRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(locations);

            // Act
            var result = await _meetingService.GetMeetingFormViewModelAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Cities.Count(), Is.EqualTo(2));
            Assert.IsTrue(result.Cities.Any(c => c.Text == "New York"));
            Assert.IsTrue(result.Cities.Any(c => c.Text == "Los Angeles"));
        }

        [Test]
        public async Task CreateMeetingAsync_CreatesMeeting_WithCorrectDetails()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var location = new Location { Id = locationId, CityName = "New York" };
            var organizer = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Organizer" };

            var meetingForm = new MeetingFormViewModel
            {
                Title = "New Meeting",
                Description = "Meeting Description",
                ScheduledDate = DateTime.UtcNow.AddDays(1),
                Duration = 2,
                LocationId = locationId
            };

            _mockLocationRepository.Setup(repo => repo.GetByIdAsync(locationId)).ReturnsAsync(location);
            _mockUserRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<ApplicationUser> { organizer }.AsQueryable().BuildMockDbSet().Object);

            _mockMeetingRepository.Setup(repo => repo.AddAsync(It.IsAny<Meeting>()))
                .Returns(Task.CompletedTask);

            // Act
            var meetingId = await _meetingService.CreateMeetingAsync(meetingForm, "Organizer");

            // Assert
            _mockMeetingRepository.Verify(repo => repo.AddAsync(It.Is<Meeting>(m =>
                m.Title == meetingForm.Title &&
                m.Description == meetingForm.Description &&
                m.ScheduledDate == meetingForm.ScheduledDate &&
                m.Duration == 2.0 &&
                m.LocationId == locationId &&
                m.Organizer.UserName == "Organizer"
            )), Times.Once);
        }

        [Test]
        public async Task UpdateMeetingAsync_UpdatesMeeting_WithCorrectDetails()
        {
            // Arrange
            var meetingId = Guid.NewGuid();
            var locationId = Guid.NewGuid();
            var meeting = new Meeting
            {
                Id = meetingId,
                Title = "Old Title",
                Description = "Old Description",
                ScheduledDate = DateTime.UtcNow,
                Duration = 1.0,
                LocationId = Guid.NewGuid()
            };
            var location = new Location { Id = locationId, CityName = "New York" };
            var updateForm = new MeetingEditViewModel
            {
                Title = "Updated Title",
                Description = "Updated Description",
                ScheduledDate = DateTime.UtcNow.AddDays(1),
                Duration = 2,
                LocationId = locationId.ToString()
            };

            _mockMeetingRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<Meeting> { meeting }.AsQueryable().BuildMockDbSet().Object);
            _mockLocationRepository.Setup(repo => repo.GetByIdAsync(locationId))
                .ReturnsAsync(location);

            // Act
            await _meetingService.UpdateMeetingAsync(meetingId, updateForm);

            // Assert
            _mockMeetingRepository.Verify(repo => repo.UpdateAsync(It.Is<Meeting>(m =>
                m.Id == meetingId &&
                m.Title == updateForm.Title &&
                m.Description == updateForm.Description &&
                m.ScheduledDate == updateForm.ScheduledDate &&
                m.Duration == updateForm.Duration &&
                m.LocationId == locationId
            )), Times.Once);
        }

        [Test]
        public async Task DeleteMeetingAsync_DeletesMeeting_WhenMeetingExists()
        {
            // Arrange
            var meetingId = Guid.NewGuid();
            _mockMeetingRepository.Setup(repo => repo.DeleteAsync(meetingId)).ReturnsAsync(true);

            // Act
            await _meetingService.DeleteMeetingAsync(meetingId);

            // Assert
            _mockMeetingRepository.Verify(repo => repo.DeleteAsync(meetingId), Times.Once);
        }

        [Test]
        public void DeleteMeetingAsync_ThrowsException_WhenMeetingDoesNotExist()
        {
            // Arrange
            var meetingId = Guid.NewGuid();
            _mockMeetingRepository.Setup(repo => repo.DeleteAsync(meetingId)).ReturnsAsync(false);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _meetingService.DeleteMeetingAsync(meetingId));
        }

        [Test]
        public async Task AttendMeetingAsync_AddsUserToAttendees_WhenValid()
        {
            // Arrange
            var meetingId = Guid.NewGuid();
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "User1" };
            var meeting = new Meeting
            {
                Id = meetingId,
                Attendees = new List<ApplicationUser>()
            };

            _mockMeetingRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<Meeting> { meeting }.AsQueryable().BuildMockDbSet().Object);

            _mockUserRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<ApplicationUser> { user }.AsQueryable().BuildMockDbSet().Object);

            // Act
            await _meetingService.AttendMeetingAsync("User1", meetingId);

            // Assert
            Assert.That(meeting.Attendees.Count, Is.EqualTo(1));
            Assert.That(meeting.Attendees.First().UserName, Is.EqualTo("User1"));
        }

        [Test]
        public async Task CancelAttendanceAsync_RemovesUserFromAttendees_WhenValid()
        {
            // Arrange
            var meetingId = Guid.NewGuid();
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "User1" };
            var meeting = new Meeting
            {
                Id = meetingId,
                Attendees = new List<ApplicationUser> { user }
            };

            _mockMeetingRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<Meeting> { meeting }.AsQueryable().BuildMockDbSet().Object);

            _mockUserRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<ApplicationUser> { user }.AsQueryable().BuildMockDbSet().Object);

            // Act
            await _meetingService.CancelAttendanceAsync("User1", meetingId);

            // Assert
            Assert.That(meeting.Attendees.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task AttendMeetingAsync_DoesNotAddDuplicateUser_WhenAlreadyAttending()
        {
            var meetingId = Guid.NewGuid();
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "User1" };
            var meeting = new Meeting
            {
                Id = meetingId,
                Attendees = new List<ApplicationUser> { user }
            };

            _mockMeetingRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<Meeting> { meeting }.AsQueryable().BuildMockDbSet().Object);

            _mockUserRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<ApplicationUser> { user }.AsQueryable().BuildMockDbSet().Object);

            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _meetingService.AttendMeetingAsync("User1", meetingId));

            Assert.That(exception.Message, Is.EqualTo("You are already attending this meeting."));

            Assert.That(meeting.Attendees.Count, Is.EqualTo(1));
        }
    }
}