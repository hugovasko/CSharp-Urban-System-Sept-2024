using Moq;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services;
using UrbanSystem.Web.ViewModels.Projects;
using MockQueryable.Moq;

namespace ServiceTests
{
    [TestFixture]
    public class ProjectServiceTests
    {
        private Mock<IRepository<Project, Guid>> _mockProjectRepository;
        private ProjectService _projectService;

        [SetUp]
        public void SetUp()
        {
            _mockProjectRepository = new Mock<IRepository<Project, Guid>>();
            _projectService = new ProjectService(_mockProjectRepository.Object);
        }

        [Test]
        public async Task GetAllProjectsAsync_ReturnsMappedViewModels_WhenProjectsExist()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Project 1",
                    FundsNeeded = 10000,
                    ImageUrl = "image1.jpg",
                    Description = "Description 1",
                    CreatedOn = DateTime.UtcNow.AddDays(-10),
                    IsCompleted = false,
                    Location = new Location { CityName = "City 1" }
                },
                new Project
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Project 2",
                    FundsNeeded = 20000,
                    ImageUrl = "image2.jpg",
                    Description = "Description 2",
                    CreatedOn = DateTime.UtcNow.AddDays(-5),
                    IsCompleted = true,
                    Location = new Location { CityName = "City 2" }
                }
            }.AsQueryable();

            _mockProjectRepository.Setup(r => r.GetAllAttached())
                .Returns(projects.BuildMockDbSet().Object);

            // Act
            var result = await _projectService.GetAllProjectsAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            var project1 = result.First();
            Assert.That(project1.Name, Is.EqualTo("Test Project 1"));
            Assert.That(project1.DesiredSum, Is.EqualTo(10000));
            Assert.That(project1.LocationName, Is.EqualTo("City 1"));
        }

        [Test]
        public async Task GetProjectByIdAsync_ReturnsProject_WhenProjectExists()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                FundsNeeded = 10000,
                ImageUrl = "image.jpg",
                Description = "Description",
                CreatedOn = DateTime.UtcNow,
                IsCompleted = false,
                Location = new Location { CityName = "City Name" }
            };

            _mockProjectRepository.Setup(r => r.GetAllAttached())
                .Returns(new List<Project> { project }.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _projectService.GetProjectByIdAsync(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(projectId));
            Assert.That(result.Name, Is.EqualTo("Test Project"));
        }

        [Test]
        public async Task GetProjectByIdAsync_ReturnsNull_WhenProjectDoesNotExist()
        {
            // Arrange
            _mockProjectRepository.Setup(r => r.GetAllAttached())
                .Returns(new List<Project>().AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _projectService.GetProjectByIdAsync(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task AddProjectAsync_AddsProject_WhenInputIsValid()
        {
            // Arrange
            var projectForm = new ProjectFormViewModel
            {
                Name = "New Project",
                DesiredSum = 15000,
                ImageUrl = "image.jpg",
                Description = "Description",
                IsCompleted = false,
                LocationId = Guid.NewGuid(),
                FundingDeadline = DateTime.UtcNow.AddMonths(1)
            };

            _mockProjectRepository.Setup(r => r.AddAsync(It.IsAny<Project>()))
                .Returns(Task.CompletedTask);

            // Act
            await _projectService.AddProjectAsync(projectForm);

            // Assert
            _mockProjectRepository.Verify(r => r.AddAsync(It.Is<Project>(p =>
                p.Name == "New Project" &&
                p.FundsNeeded == 15000 &&
                p.ImageUrl == "image.jpg" &&
                p.Description == "Description")), Times.Once);
        }

        [Test]
        public void AddProjectAsync_ThrowsException_WhenProjectIsNull()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _projectService.AddProjectAsync(null!));
        }

        [Test]
        public async Task DeleteProjectAsync_ReturnsTrue_WhenProjectExists()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project { Id = projectId };

            _mockProjectRepository.Setup(r => r.GetAllAttached())
                .Returns(new List<Project> { project }.AsQueryable().BuildMockDbSet().Object);
            _mockProjectRepository.Setup(r => r.DeleteAsync(projectId))
                .ReturnsAsync(true);

            // Act
            var result = await _projectService.DeleteProjectAsync(projectId);

            // Assert
            Assert.IsTrue(result);
            _mockProjectRepository.Verify(r => r.DeleteAsync(projectId), Times.Once);
        }

        [Test]
        public async Task DeleteProjectAsync_ReturnsFalse_WhenProjectDoesNotExist()
        {
            // Arrange
            _mockProjectRepository.Setup(r => r.GetAllAttached())
                .Returns(new List<Project>().AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _projectService.DeleteProjectAsync(Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }
    }
}
