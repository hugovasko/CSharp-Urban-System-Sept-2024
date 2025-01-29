using Moq;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data;
using UrbanSystem.Web.ViewModels.Meetings;
using MockQueryable.Moq;

namespace ServiceTests
{
    [TestFixture]
    public class MeetingManagementServiceTests
    {
        private Mock<IRepository<Meeting, Guid>> _mockMeetingRepository;
        private Mock<IRepository<Location, Guid>> _mockLocationRepository;
        private Mock<IRepository<ApplicationUser, string>> _mockUserRepository;
        private MeetingManagementService _service;

        [SetUp]
        public void Setup()
        {
            // Initialize mocks
            _mockMeetingRepository = new Mock<IRepository<Meeting, Guid>>();
            _mockLocationRepository = new Mock<IRepository<Location, Guid>>();
            _mockUserRepository = new Mock<IRepository<ApplicationUser, string>>();

            // Initialize service
            _service = new MeetingManagementService(
                _mockMeetingRepository.Object,
                _mockLocationRepository.Object,
                _mockUserRepository.Object
            );
        }

        [Test]
        public async Task GetAllMeetingsAsync_ReturnsCorrectViewModels()
        {
            // Arrange
            var mockMeetings = new List<Meeting>
            {
                new Meeting
                {
                    Id = Guid.NewGuid(),
                    Title = "Meeting A",
                    Description = "Description A",
                    ScheduledDate = DateTime.UtcNow,
                    Duration = 2.0,
                    Location = new Location { CityName = "City A" },
                    Attendees = new List<ApplicationUser> { new ApplicationUser { UserName = "User1" } },
                    Organizer = new ApplicationUser { UserName = "Organizer1" }
                },
                new Meeting
                {
                    Id = Guid.NewGuid(),
                    Title = "Meeting B",
                    Description = "Description B",
                    ScheduledDate = DateTime.UtcNow.AddDays(1),
                    Duration = 1.0,
                    Location = new Location { CityName = "City B" },
                    Attendees = new List<ApplicationUser> { new ApplicationUser { UserName = "User2" } },
                    Organizer = new ApplicationUser { UserName = "Organizer2" }
                }
            };

            _mockMeetingRepository
                .Setup(repo => repo.GetAllAttached())
                .Returns(mockMeetings.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _service.GetAllMeetingsAsync();

            // Assert
            Assert.IsNotNull(result);
            var meetings = result.ToList();
            Assert.AreEqual(2, meetings.Count);

            Assert.AreEqual("Meeting A", meetings[0].Title);
            Assert.AreEqual("City A", meetings[0].CityName);
            Assert.AreEqual(1, meetings[0].AttendeesCount);
            Assert.AreEqual("Organizer1", meetings[0].OrganizerName);

            Assert.AreEqual("Meeting B", meetings[1].Title);
            Assert.AreEqual("City B", meetings[1].CityName);
            Assert.AreEqual(1, meetings[1].AttendeesCount);
            Assert.AreEqual("Organizer2", meetings[1].OrganizerName);

            _mockMeetingRepository.Verify(repo => repo.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task DeleteMeetingAsync_DeletesCorrectly()
        {
            // Arrange
            var meetingId = Guid.NewGuid();
            _mockMeetingRepository
                .Setup(repo => repo.DeleteAsync(meetingId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteMeetingAsync(meetingId);

            // Assert
            Assert.IsTrue(result);

            _mockMeetingRepository.Verify(repo => repo.DeleteAsync(meetingId), Times.Once);
        }

        [Test]
        public async Task CreateMeetingAsync_CreatesCorrectly()
        {
            // Arrange
            var mockMeetingForm = new MeetingFormViewModel
            {
                Title = "New Meeting",
                Description = "Description of the new meeting",
                ScheduledDate = DateTime.UtcNow,
                Duration = 2,
                LocationId = Guid.NewGuid()
            };

            _mockMeetingRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Meeting>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateMeetingAsync(mockMeetingForm);

            // Assert
            Assert.IsTrue(result);
            _mockMeetingRepository.Verify(repo => repo.AddAsync(It.IsAny<Meeting>()), Times.Once);
        }

        [Test]
        public async Task GetAllCitiesAsync_ReturnsCorrectCities()
        {
            // Arrange
            var mockLocations = new List<Location>
            {
                new Location { Id = Guid.NewGuid(), CityName = "City A" },
                new Location { Id = Guid.NewGuid(), CityName = "City B" }
            };

            _mockLocationRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(mockLocations);

            // Act
            var result = await _service.GetAllCitiesAsync();

            // Assert
            Assert.IsNotNull(result);
            var cities = result.ToList();
            Assert.AreEqual(2, cities.Count);

            Assert.AreEqual("City A", cities[0].Text);
            Assert.AreEqual("City B", cities[1].Text);

            _mockLocationRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }
    }
}
