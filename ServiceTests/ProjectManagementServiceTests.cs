using MockQueryable.Moq;
using Moq;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data;

namespace ServiceTests
{
    [TestFixture]
    public class ProjectManagementServiceTests
    {
        private Mock<IRepository<Project, Guid>> _mockProjectRepository;
        private Mock<IRepository<Location, Guid>> _mockLocationRepository;
        private ProjectManagementService _service;

        [SetUp]
        public void Setup()
        {
            // Initialize mocks
            _mockProjectRepository = new Mock<IRepository<Project, Guid>>();
            _mockLocationRepository = new Mock<IRepository<Location, Guid>>();

            // Initialize service
            _service = new ProjectManagementService(
                _mockProjectRepository.Object,
                _mockLocationRepository.Object
            );
        }

        [Test]
        public async Task GetAllProjectsAsync_ReturnsCorrectViewModels()
        {
            // Arrange
            var mockProjects = new List<Project>
            {
                new Project
                {
                    Id = Guid.NewGuid(),
                    Name = "Project A",
                    FundsNeeded = 5000,
                    ImageUrl = "imageA.jpg",
                    Description = "Description A",
                    CreatedOn = DateTime.UtcNow,
                    IsCompleted = false,
                    Location = new Location { CityName = "City A" }
                },
                new Project
                {
                    Id = Guid.NewGuid(),
                    Name = "Project B",
                    FundsNeeded = 10000,
                    ImageUrl = "imageB.jpg",
                    Description = "Description B",
                    CreatedOn = DateTime.UtcNow.AddDays(-1),
                    IsCompleted = true,
                    Location = new Location { CityName = "City B" }
                }
            };

            _mockProjectRepository
                .Setup(repo => repo.GetAllAttached())
                .Returns(mockProjects.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _service.GetAllProjectsAsync();

            // Assert
            Assert.IsNotNull(result);
            var projects = result.ToList();
            Assert.That(projects.Count, Is.EqualTo(2));

            Assert.That(projects[0].Name, Is.EqualTo("Project A"));
            Assert.That(projects[0].DesiredSum, Is.EqualTo(5000));
            Assert.That(projects[0].LocationName, Is.EqualTo("City A"));

            Assert.That(projects[1].Name, Is.EqualTo("Project B"));
            Assert.That(projects[1].DesiredSum, Is.EqualTo(10000));
            Assert.That(projects[1].LocationName, Is.EqualTo("City B"));

            _mockProjectRepository.Verify(repo => repo.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetProjectByIdAsync_ReturnsCorrectViewModel()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var mockProject = new Project
            {
                Id = projectId,
                Name = "Project A",
                FundsNeeded = 5000,
                ImageUrl = "imageA.jpg",
                Description = "Description A",
                CreatedOn = DateTime.UtcNow,
                IsCompleted = false,
                Location = new Location { CityName = "City A" }
            };

            _mockProjectRepository
                .Setup(repo => repo.GetAllAttached())
                .Returns(new List<Project> { mockProject }.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _service.GetProjectByIdAsync(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("Project A"));
            Assert.That(result.LocationName, Is.EqualTo("City A"));

            _mockProjectRepository.Verify(repo => repo.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task UpdateProjectCompletionAsync_UpdatesIsCompleted()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var mockProject = new Project
            {
                Id = projectId,
                IsCompleted = false
            };

            _mockProjectRepository
                .Setup(repo => repo.GetAllAttached())
                .Returns(new List<Project> { mockProject }.AsQueryable().BuildMockDbSet().Object);

            _mockProjectRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<Project>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateProjectCompletionAsync(projectId, true);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(mockProject.IsCompleted);

            _mockProjectRepository.Verify(repo => repo.UpdateAsync(mockProject), Times.Once);
        }

        [Test]
        public async Task DeleteProjectAsync_DeletesProject()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            _mockProjectRepository
                .Setup(repo => repo.DeleteAsync(projectId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteProjectAsync(projectId);

            // Assert
            Assert.IsTrue(result);

            _mockProjectRepository.Verify(repo => repo.DeleteAsync(projectId), Times.Once);
        }

        [Test]
        public async Task ToggleProjectCompletionAsync_TogglesIsCompleted()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var mockProject = new Project
            {
                Id = projectId,
                IsCompleted = false
            };

            _mockProjectRepository
                .Setup(repo => repo.GetByIdAsync(projectId))
                .ReturnsAsync(mockProject);

            _mockProjectRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<Project>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.ToggleProjectCompletionAsync(projectId);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(mockProject.IsCompleted);

            _mockProjectRepository.Verify(repo => repo.UpdateAsync(mockProject), Times.Once);
        }
    }
}
