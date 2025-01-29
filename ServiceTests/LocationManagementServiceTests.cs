using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data;
using UrbanSystem.Web.ViewModels.Locations;

namespace UrbanSystem.Services.Tests
{
    [TestFixture]
    public class LocationManagementServiceTests
    {
        private Mock<IRepository<Location, Guid>> _locationRepositoryMock;
        private Mock<IRepository<Suggestion, Guid>> _suggestionRepositoryMock;
        private Mock<IRepository<Meeting, Guid>> _meetingRepositoryMock;
        private Mock<IRepository<Project, Guid>> _projectRepositoryMock;
        private Mock<IRepository<SuggestionLocation, object>> _suggestionLocationRepositoryMock;
        private LocationManagementService _service;

        [SetUp]
        public void SetUp()
        {
            _locationRepositoryMock = new Mock<IRepository<Location, Guid>>();
            _suggestionRepositoryMock = new Mock<IRepository<Suggestion, Guid>>();
            _meetingRepositoryMock = new Mock<IRepository<Meeting, Guid>>();
            _projectRepositoryMock = new Mock<IRepository<Project, Guid>>();
            _suggestionLocationRepositoryMock = new Mock<IRepository<SuggestionLocation, object>>();

            _service = new LocationManagementService(
                _locationRepositoryMock.Object,
                _suggestionRepositoryMock.Object,
                _meetingRepositoryMock.Object,
                _projectRepositoryMock.Object,
                _suggestionLocationRepositoryMock.Object
            );
        }

        [Test]
        public async Task GetAllLocationsAsync_ReturnsMappedLocations()
        {
            // Arrange
            var locations = new List<Location>
            {
                new Location { Id = Guid.NewGuid(), CityName = "City1", StreetName = "Street1" },
                new Location { Id = Guid.NewGuid(), CityName = "City2", StreetName = "Street2" }
            };
            _locationRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(locations);

            // Act
            var result = await _service.GetAllLocationsAsync();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("City1", result.First().CityName);
            Assert.AreEqual("Street1", result.First().StreetName);
        }

        [Test]
        public async Task GetLocationByIdAsync_ValidId_ReturnsLocation()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var location = new Location { Id = locationId, CityName = "City1", StreetName = "Street1" };
            _locationRepositoryMock.Setup(repo => repo.GetByIdAsync(locationId)).ReturnsAsync(location);

            // Act
            var result = await _service.GetLocationByIdAsync(locationId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(locationId, result.Id);
            Assert.AreEqual("City1", result.CityName);
        }

        [Test]
        public async Task DeleteLocationAsync_ValidId_ReturnsTrue()
        {
            var locationId = Guid.NewGuid();
            _locationRepositoryMock.Setup(repo => repo.DeleteAsync(locationId)).ReturnsAsync(true);

            var result = await _service.DeleteLocationAsync(locationId);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task LocationExistsByIdAsync_ValidId_ReturnsTrue()
        {
            var locationId = Guid.NewGuid();
            var location = new Location { Id = locationId };
            _locationRepositoryMock.Setup(repo => repo.GetByIdAsync(locationId)).ReturnsAsync(location);

            var result = await _service.LocationExistsByIdAsync(locationId);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteSuggestionsByLocationIdAsync_DeletesAssociatedSuggestions()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var suggestionLocations = new List<SuggestionLocation>
            {
                new SuggestionLocation { SuggestionId = Guid.NewGuid(), LocationId = locationId },
                new SuggestionLocation { SuggestionId = Guid.NewGuid(), LocationId = locationId }
            };
            var orphanedSuggestions = new List<Suggestion>
            {
                new Suggestion { Id = Guid.NewGuid(), SuggestionsLocations = new List<SuggestionLocation>() }
            };

            _suggestionLocationRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<SuggestionLocation, bool>>>()))
                    .ReturnsAsync(suggestionLocations);

            _suggestionRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Suggestion, bool>>>()))
                    .ReturnsAsync(orphanedSuggestions);

            _suggestionRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>()));

            // Act
            await _service.DeleteSuggestionsByLocationIdAsync(locationId);

            // Assert
            _suggestionLocationRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<object[]>()), Times.Exactly(2));
            _suggestionRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
