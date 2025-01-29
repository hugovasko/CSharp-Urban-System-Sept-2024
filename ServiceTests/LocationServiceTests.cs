using Moq;
using MockQueryable.Moq;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data;
using UrbanSystem.Web.ViewModels.Locations;

namespace ServiceTests
{
    [TestFixture]
    public class LocationServiceTests
    {
        private Mock<IRepository<Location, Guid>> _mockLocationRepository;
        private LocationService _locationService;

        [SetUp]
        public void Setup()
        {
            _mockLocationRepository = new Mock<IRepository<Location, Guid>>();
            _locationService = new LocationService(_mockLocationRepository.Object);
        }

        [Test]
        public async Task AddLocationAsync_CallsRepositoryWithCorrectLocation()
        {
            // Arrange
            var model = new LocationFormViewModel
            {
                CityName = "New York",
                StreetName = "5th Avenue",
                CityPicture = "image.jpg"
            };

            Location? savedLocation = null;

            _mockLocationRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Location>()))
                .Callback<Location>(loc => savedLocation = loc)
                .Returns(Task.CompletedTask);

            // Act
            await _locationService.AddLocationAsync(model);

            // Assert
            _mockLocationRepository.Verify(repo => repo.AddAsync(It.IsAny<Location>()), Times.Once);
            Assert.IsNotNull(savedLocation);
            Assert.That(savedLocation.CityName, Is.EqualTo(model.CityName));
            Assert.That(savedLocation.StreetName, Is.EqualTo(model.StreetName));
            Assert.That(savedLocation.CityPicture, Is.EqualTo(model.CityPicture));
        }

        [Test]
        public async Task GetAllOrderedByNameAsync_ReturnsCorrectlyOrderedAndPaginatedLocations()
        {
            // Arrange
            var testLocations = new List<Location>
            {
                new Location { Id = Guid.NewGuid(), CityName = "Chicago", StreetName = "Main Street", CityPicture = "chicago.jpg" },
                new Location { Id = Guid.NewGuid(), CityName = "New York", StreetName = "5th Avenue", CityPicture = "newyork.jpg" },
                new Location { Id = Guid.NewGuid(), CityName = "Los Angeles", StreetName = "Sunset Blvd", CityPicture = "la.jpg" },
                new Location { Id = Guid.NewGuid(), CityName = "Boston", StreetName = "Beacon Street", CityPicture = "boston.jpg" },
                new Location { Id = Guid.NewGuid(), CityName = "Miami", StreetName = "Ocean Drive", CityPicture = "miami.jpg" }
            };

            _mockLocationRepository.Setup(repo => repo.GetAllAttached())
                .Returns(testLocations.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _locationService.GetAllOrderedByNameAsync(1, 2);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].CityName, Is.EqualTo("Boston"));
            Assert.That(result[1].CityName, Is.EqualTo("Chicago"));
            Assert.That(result.TotalPages, Is.EqualTo(3));
            Assert.That(result.HasPreviousPage, Is.False);
            Assert.That(result.HasNextPage, Is.True);
        }

        [Test]
        public async Task GetAllOrderedByNameAsync_ReturnsLastPage()
        {
            // Arrange
            var testLocations = new List<Location>
            {
                new Location { Id = Guid.NewGuid(), CityName = "Chicago", StreetName = "Main Street", CityPicture = "chicago.jpg" },
                new Location { Id = Guid.NewGuid(), CityName = "New York", StreetName = "5th Avenue", CityPicture = "newyork.jpg" },
                new Location { Id = Guid.NewGuid(), CityName = "Los Angeles", StreetName = "Sunset Blvd", CityPicture = "la.jpg" },
                new Location { Id = Guid.NewGuid(), CityName = "Boston", StreetName = "Beacon Street", CityPicture = "boston.jpg" },
                new Location { Id = Guid.NewGuid(), CityName = "Miami", StreetName = "Ocean Drive", CityPicture = "miami.jpg" }
            };

            _mockLocationRepository.Setup(repo => repo.GetAllAttached())
                .Returns(testLocations.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _locationService.GetAllOrderedByNameAsync(3, 2);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].CityName, Is.EqualTo("New York"));
            Assert.That(result.TotalPages, Is.EqualTo(3));
            Assert.That(result.HasPreviousPage, Is.True);
            Assert.That(result.HasNextPage, Is.False);
        }

        [Test]
        public async Task GetLocationDetailsByIdAsync_ReturnsNull_WhenIdIsInvalid()
        {
            // Act
            var result = await _locationService.GetLocationDetailsByIdAsync("invalid-guid");

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetLocationDetailsByIdAsync_ReturnsNull_WhenLocationDoesNotExist()
        {
            // Arrange
            var validId = Guid.NewGuid().ToString();
            _mockLocationRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<Location>().AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _locationService.GetLocationDetailsByIdAsync(validId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetLocationDetailsByIdAsync_ReturnsCorrectDetails_WhenLocationExists()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var suggestionId = Guid.NewGuid();

            var testLocation = new Location
            {
                Id = locationId,
                CityName = "New York",
                StreetName = "5th Avenue",
                CityPicture = "image.jpg",
                SuggestionsLocations = new List<SuggestionLocation>
                {
                    new SuggestionLocation
                    {
                        Suggestion = new Suggestion { Id = suggestionId, Title = "Suggestion 1" }
                    }
                }
            };

            _mockLocationRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<Location> { testLocation }.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _locationService.GetLocationDetailsByIdAsync(locationId.ToString());

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(locationId.ToString()));
            Assert.That(result.CityName, Is.EqualTo("New York"));
            Assert.That(result.StreetName, Is.EqualTo("5th Avenue"));
            Assert.That(result.CityPicture, Is.EqualTo("image.jpg"));
            Assert.That(result.Suggestions.Count(), Is.EqualTo(1));
            Assert.That(result.Suggestions.First().Id, Is.EqualTo(suggestionId.ToString()));
            Assert.That(result.Suggestions.First().Title, Is.EqualTo("Suggestion 1"));
        }

        [Test]
        public async Task GetAllOrderedByNameAsync_ReturnsEmptyList_WhenNoLocationsExist()
        {
            // Arrange
            _mockLocationRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<Location>().AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _locationService.GetAllOrderedByNameAsync(1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
            Assert.That(result.TotalPages, Is.EqualTo(0));
            Assert.That(result.HasPreviousPage, Is.False);
            Assert.That(result.HasNextPage, Is.False);
        }

        [Test]
        public async Task GetAllOrderedByNameAsync_ReturnsCorrectPage_WhenPageNumberExceedsTotalPages()
        {
            // Arrange
            var testLocations = new List<Location>
            {
                new Location { Id = Guid.NewGuid(), CityName = "Chicago", StreetName = "Main Street", CityPicture = "chicago.jpg" },
                new Location { Id = Guid.NewGuid(), CityName = "New York", StreetName = "5th Avenue", CityPicture = "newyork.jpg" },
            };

            _mockLocationRepository.Setup(repo => repo.GetAllAttached())
                .Returns(testLocations.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _locationService.GetAllOrderedByNameAsync(3, 1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
            Assert.That(result.TotalPages, Is.EqualTo(2));
            Assert.That(result.HasPreviousPage, Is.True);
            Assert.That(result.HasNextPage, Is.False);
        }
    }
}