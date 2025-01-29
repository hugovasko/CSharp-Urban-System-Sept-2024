using Moq;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data;

namespace ServiceTests
{
    [TestFixture]
    public class BaseServiceTests
    {
        private Mock<IRepository<Location, Guid>> _mockLocationRepository;
        private BaseService _baseService;

        [SetUp]
        public void Setup()
        {
            _mockLocationRepository = new Mock<IRepository<Location, Guid>>();
            _baseService = new BaseService(_mockLocationRepository.Object);
        }

        [Test]
        public async Task GetCitiesAsync_ReturnsCorrectCityOptions()
        {
            // Arrange
            var testLocations = new List<Location>
            {
                new Location { Id = Guid.NewGuid(), CityName = "New York" },
                new Location { Id = Guid.NewGuid(), CityName = "Los Angeles" },
                new Location { Id = Guid.NewGuid(), CityName = "Chicago" }
            };

            _mockLocationRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(testLocations);

            // Act
            var result = await _baseService.GetCitiesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.IsTrue(result.All(c => testLocations.Any(l => l.Id.ToString() == c.Value && l.CityName == c.Text)));
        }

        [Test]
        public async Task GetCitiesAsync_ReturnsEmptySet_WhenNoLocationsExist()
        {
            // Arrange
            _mockLocationRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Location>());

            // Act
            var result = await _baseService.GetCitiesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetCitiesAsync_ReturnsUniqueSet_WhenDuplicateCitiesExist()
        {
            // Arrange
            var testLocations = new List<Location>
            {
                new Location { Id = Guid.NewGuid(), CityName = "New York" },
                new Location { Id = Guid.NewGuid(), CityName = "New York" },
                new Location { Id = Guid.NewGuid(), CityName = "Chicago" }
            };

            _mockLocationRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(testLocations);

            // Act
            var result = await _baseService.GetCitiesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.IsTrue(result.Any(c => c.Text == "New York"));
            Assert.IsTrue(result.Any(c => c.Text == "Chicago"));
        }

        [Test]
        public async Task GetCitiesAsync_PreservesFirstIdForDuplicateCities()
        {
            // Arrange
            var newYorkId1 = Guid.NewGuid();
            var newYorkId2 = Guid.NewGuid();
            var testLocations = new List<Location>
            {
                new Location { Id = newYorkId1, CityName = "New York" },
                new Location { Id = newYorkId2, CityName = "New York" },
                new Location { Id = Guid.NewGuid(), CityName = "Chicago" }
            };

            _mockLocationRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(testLocations);

            // Act
            var result = await _baseService.GetCitiesAsync();

            // Assert
            Assert.IsNotNull(result);
            var newYorkOption = result.FirstOrDefault(c => c.Text == "New York");
            Assert.IsNotNull(newYorkOption);
            Assert.That(newYorkOption.Value, Is.EqualTo(newYorkId1.ToString()));
        }

        [Test]
        public async Task GetCitiesAsync_HandlesNullCityNames()
        {
            // Arrange
            var testLocations = new List<Location>
            {
                new Location { Id = Guid.NewGuid(), CityName = "New York" },
                new Location { Id = Guid.NewGuid(), CityName = null! },
                new Location { Id = Guid.NewGuid(), CityName = "Chicago" }
            };

            _mockLocationRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(testLocations);

            // Act
            var result = await _baseService.GetCitiesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.IsTrue(result.All(c => c.Text != null));
        }

        [Test]
        public async Task GetCitiesAsync_PreservesOrderOfCities()
        {
            // Arrange
            var testLocations = new List<Location>
            {
                new Location { Id = Guid.NewGuid(), CityName = "Chicago" },
                new Location { Id = Guid.NewGuid(), CityName = "New York" },
                new Location { Id = Guid.NewGuid(), CityName = "Los Angeles" }
            };

            _mockLocationRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(testLocations);

            // Act
            var result = await _baseService.GetCitiesAsync();

            // Assert
            Assert.IsNotNull(result);
            var resultList = result.ToList();
            Assert.That(resultList[0].Text, Is.EqualTo("Chicago"));
            Assert.That(resultList[1].Text, Is.EqualTo("New York"));
            Assert.That(resultList[2].Text, Is.EqualTo("Los Angeles"));
        }
    }
}