using Moq;
using Microsoft.AspNetCore.Identity;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data;
using UrbanSystem.Web.ViewModels.Suggestions;
using MockQueryable;
using MockQueryable.Moq;

namespace ServiceTests
{
    [TestFixture]
    public class SuggestionServiceTests
    {
        private Mock<IRepository<Suggestion, Guid>> _mockSuggestionRepository;
        private Mock<IRepository<Location, Guid>> _mockLocationRepository;
        private Mock<IRepository<ApplicationUserSuggestion, object>> _mockUserSuggestionRepository;
        private Mock<IRepository<SuggestionLocation, object>> _mockSuggestionLocationRepository;
        private Mock<IRepository<Comment, Guid>> _mockCommentRepository;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private SuggestionService _suggestionService;

        // Helper method to mock UserManager<ApplicationUser>
        private static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mock = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            return mock;
        }

        [SetUp]
        public void SetUp()
        {
            _mockSuggestionRepository = new Mock<IRepository<Suggestion, Guid>>();
            _mockLocationRepository = new Mock<IRepository<Location, Guid>>();
            _mockUserSuggestionRepository = new Mock<IRepository<ApplicationUserSuggestion, object>>();
            _mockSuggestionLocationRepository = new Mock<IRepository<SuggestionLocation, object>>();
            _mockCommentRepository = new Mock<IRepository<Comment, Guid>>();

            _mockUserManager = MockUserManager(); // Mock the UserManager<ApplicationUser>

            _suggestionService = new SuggestionService(
                _mockSuggestionRepository.Object,
                _mockLocationRepository.Object,
                _mockUserSuggestionRepository.Object,
                _mockSuggestionLocationRepository.Object,
                _mockCommentRepository.Object,
                _mockUserManager.Object
            );
        }

        [Test]
        public async Task GetAllSuggestionsAsync_ReturnsPaginatedSuggestions()
        {
            // Arrange
            var suggestions = new List<Suggestion>
            {
                new Suggestion { Id = Guid.NewGuid(), Title = "Suggestion 1", Category = "Category 1", Description = "Description 1", UploadedOn = DateTime.UtcNow },
                new Suggestion { Id = Guid.NewGuid(), Title = "Suggestion 2", Category = "Category 2", Description = "Description 2", UploadedOn = DateTime.UtcNow }
            };

            var mockDbSet = suggestions.AsQueryable().BuildMockDbSet();
            _mockSuggestionRepository.Setup(r => r.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _suggestionService.GetAllSuggestionsAsync(1, 10);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Title, Is.EqualTo("Suggestion 1"));
            Assert.That(result.TotalPages, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllSuggestionsAsync_WithSearchQuery_ReturnsPaginatedFilteredSuggestions()
        {
            // Arrange
            var suggestions = new List<Suggestion>
            {
                new Suggestion { Id = Guid.NewGuid(), Title = "Suggestion 1", Category = "Category 1", Description = "Description 1", UploadedOn = DateTime.UtcNow },
                new Suggestion { Id = Guid.NewGuid(), Title = "Suggestion 2", Category = "Category 2", Description = "Description 2", UploadedOn = DateTime.UtcNow }
            };

            var mockDbSet = suggestions.AsQueryable().BuildMockDbSet();
            _mockSuggestionRepository.Setup(r => r.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _suggestionService.GetAllSuggestionsAsync(1, 10, "Suggestion 1");

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Suggestion 1"));
            Assert.That(result.TotalPages, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllSuggestionsAsync_WithSorting_ReturnsPaginatedSortedSuggestions()
        {
            // Arrange
            var suggestions = new List<Suggestion>
            {
                new Suggestion { Id = Guid.NewGuid(), Title = "B Suggestion", Category = "Category 1", Description = "Description 1", UploadedOn = DateTime.UtcNow },
                new Suggestion { Id = Guid.NewGuid(), Title = "A Suggestion", Category = "Category 2", Description = "Description 2", UploadedOn = DateTime.UtcNow.AddDays(-1) }
            };

            var mockDbSet = suggestions.AsQueryable().BuildMockDbSet();
            _mockSuggestionRepository.Setup(r => r.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _suggestionService.GetAllSuggestionsAsync(1, 10, sortBy: "Title", ascending: true);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Title, Is.EqualTo("A Suggestion"));
            Assert.That(result[1].Title, Is.EqualTo("B Suggestion"));
        }

        [Test]
        public async Task AddSuggestionAsync_ValidUser_Success()
        {
            // Arrange
            var model = new SuggestionFormViewModel
            {
                Title = "New Suggestion",
                Category = "Category",
                Description = "Description",
                AttachmentUrl = "http://example.com",
                Status = "Open",
                Priority = "High",
                CityId = Guid.NewGuid() // ensure this is a valid city ID
            };

            var userId = Guid.NewGuid(); // valid user ID

            // Mock UserManager to return a valid user
            var user = new ApplicationUser { Id = userId, UserName = "testuser" };
            _mockUserManager.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);

            // Mock Location Repository to return a valid location for CityId
            var location = new Location { Id = Guid.NewGuid(), CityName = "Test City" };
            _mockLocationRepository.Setup(repo => repo.GetByIdAsync(model.CityId)).ReturnsAsync(location);

            // Mock Suggestion Repository to confirm AddAsync is called
            _mockSuggestionRepository.Setup(repo => repo.AddAsync(It.IsAny<Suggestion>())).Returns(Task.CompletedTask);

            // Mock UserSuggestion Repository to confirm AddAsync is called
            _mockUserSuggestionRepository.Setup(repo => repo.AddAsync(It.IsAny<ApplicationUserSuggestion>())).Returns(Task.CompletedTask);

            // Mock SuggestionLocation Repository to confirm AddAsync is called
            _mockSuggestionLocationRepository.Setup(repo => repo.AddAsync(It.IsAny<SuggestionLocation>())).Returns(Task.CompletedTask);

            // Act
            var result = await _suggestionService.AddSuggestionAsync(model, userId.ToString());

            // Assert
            Assert.IsTrue(result.IsSuccessful, "Expected successful result but got failure.");
            Assert.IsNull(result.ErrorMessage); // Ensure there are no error messages
            _mockSuggestionRepository.Verify(repo => repo.AddAsync(It.IsAny<Suggestion>()), Times.Once);
            _mockUserSuggestionRepository.Verify(repo => repo.AddAsync(It.IsAny<ApplicationUserSuggestion>()), Times.Once);
            _mockSuggestionLocationRepository.Verify(repo => repo.AddAsync(It.IsAny<SuggestionLocation>()), Times.Once);
        }

        [Test]
        public void SortSuggestions_SortByTitle_Ascending_Success()
        {
            // Arrange
            var suggestions = new List<SuggestionIndexViewModel>
            {
                new SuggestionIndexViewModel { Title = "Zebra", UploadedOn = DateTime.UtcNow.ToString() },
                new SuggestionIndexViewModel { Title = "Apple", UploadedOn = DateTime.UtcNow.AddDays(-1).ToString() }
            };
            var sortBy = "Title";
            var ascending = true;

            // Act
            var result = _suggestionService.SortSuggestions(suggestions, sortBy, ascending);

            // Assert
            Assert.That(result.First().Title, Is.EqualTo("Apple"));
            Assert.That(result.Last().Title, Is.EqualTo("Zebra"));
        }

        [Test]
        public void SortSuggestions_SortByDate_Descending_Success()
        {
            // Arrange
            var suggestions = new List<SuggestionIndexViewModel>
            {
                new SuggestionIndexViewModel { Title = "Zebra", UploadedOn = DateTime.UtcNow.ToString() },
                new SuggestionIndexViewModel { Title = "Apple", UploadedOn = DateTime.UtcNow.AddDays(-1).ToString() }
            };
            var sortBy = "Date";
            var ascending = false;

            // Act
            var result = _suggestionService.SortSuggestions(suggestions, sortBy, ascending);

            // Assert
            Assert.That(result.First().Title, Is.EqualTo("Zebra"));
            Assert.That(result.Last().Title, Is.EqualTo("Apple"));
        }

        [Test]
        public async Task AddSuggestionAsync_InvalidUserId_ReturnsErrorMessage()
        {
            // Arrange
            var model = new SuggestionFormViewModel
            {
                Title = "New Suggestion",
                Category = "Category",
                Description = "Description",
                AttachmentUrl = "http://example.com",
                Status = "Open",
                Priority = "High",
                CityId = Guid.NewGuid()
            };

            // Act
            var result = await _suggestionService.AddSuggestionAsync(model, "invalid-user-id");

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid user ID."));
        }

        [Test]
        public async Task AddSuggestionAsync_UserNotFound_ReturnsErrorMessage()
        {
            // Arrange
            var model = new SuggestionFormViewModel
            {
                Title = "New Suggestion",
                Category = "Category",
                Description = "Description",
                AttachmentUrl = "http://example.com",
                Status = "Open",
                Priority = "High",
                CityId = Guid.NewGuid()
            };

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _suggestionService.AddSuggestionAsync(model, "user-id-not-found");

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid user ID."));
        }

        [Test]
        public async Task GetSuggestionDetailsAsync_InvalidId_ReturnsErrorMessage()
        {
            // Arrange
            string invalidSuggestionId = "invalid-id";
            string userId = Guid.NewGuid().ToString();

            // Act
            var result = await _suggestionService.GetSuggestionDetailsAsync(invalidSuggestionId, userId);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid suggestion ID."));
        }

        [Test]
        public async Task GetSuggestionDetailsAsync_SuggestionNotFound_ReturnsErrorMessage()
        {
            // Arrange
            var validSuggestionId = Guid.NewGuid().ToString();
            string userId = Guid.NewGuid().ToString();

            _mockSuggestionRepository.Setup(r => r.GetAllAttached())
                .Returns(new List<Suggestion>().AsQueryable().BuildMock()); // Empty list

            // Act
            var result = await _suggestionService.GetSuggestionDetailsAsync(validSuggestionId, userId);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.That(result.ErrorMessage, Is.EqualTo("Suggestion not found."));
        }

        [Test]
        public async Task AddCommentAsync_ValidComment_Success()
        {
            // Arrange
            var suggestionId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();
            var content = "This is a test comment.";

            var user = new ApplicationUser { Id = userId, UserName = "TestUser" };
            _mockUserManager.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);

            // Act
            var result = await _suggestionService.AddCommentAsync(suggestionId, content, userId.ToString());

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            _mockCommentRepository.Verify(repo => repo.AddAsync(It.IsAny<Comment>()), Times.Once);
        }

        [Test]
        public async Task AddCommentAsync_InvalidSuggestionId_ReturnsErrorMessage()
        {
            // Arrange
            string invalidSuggestionId = "invalid-id";
            string content = "Great suggestion!";
            string userId = Guid.NewGuid().ToString();

            // Act
            var result = await _suggestionService.AddCommentAsync(invalidSuggestionId, content, userId);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid suggestion ID."));
        }

        [Test]
        public async Task AddCommentAsync_UserNotFound_ReturnsErrorMessage()
        {
            // Arrange
            string suggestionId = Guid.NewGuid().ToString();
            string content = "Great suggestion!";
            string userId = Guid.NewGuid().ToString();

            _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _suggestionService.AddCommentAsync(suggestionId, content, userId);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.That(result.ErrorMessage, Is.EqualTo("User not found."));
        }

        [Test]
        public async Task GetSuggestionFormViewModelAsync_ReturnsViewModel()
        {
            // Arrange
            var cities = new List<Location>
        {
            new Location { Id = Guid.NewGuid(), CityName = "City 1" },
            new Location { Id = Guid.NewGuid(), CityName = "City 2" }
        };
            _mockLocationRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(cities);

            // Act
            var result = await _suggestionService.GetSuggestionFormViewModelAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Cities.Count, Is.EqualTo(2));
            Assert.That(result.Cities.First().Text, Is.EqualTo("City 1"));
        }

        [Test]
        public async Task GetCommentAsync_ValidCommentId_ReturnsCommentViewModel()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var comment = new Comment
            {
                Id = commentId,
                Content = "Test Comment",
                AddedOn = DateTime.UtcNow,
                User = new ApplicationUser { UserName = "TestUser" }
            };
            _mockCommentRepository.Setup(r => r.GetAllAttached())
                .Returns(new List<Comment> { comment }.AsQueryable().BuildMock());

            // Act
            var result = await _suggestionService.GetCommentAsync(commentId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Content, Is.EqualTo("Test Comment"));
            Assert.That(result.UserName, Is.EqualTo("TestUser"));
        }

        [Test]
        public async Task GetSuggestionForEditAsync_ValidSuggestionAndUser_ReturnsViewModel()
        {
            // Arrange
            var suggestionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var suggestion = new Suggestion
            {
                Id = suggestionId,
                Title = "Test Suggestion",
                Category = "Test Category",
                Description = "Test Description",
                UsersSuggestions = new List<ApplicationUserSuggestion>
            {
                new ApplicationUserSuggestion { ApplicationUserId = userId }
            },
                SuggestionsLocations = new List<SuggestionLocation>
            {
                new SuggestionLocation { LocationId = Guid.NewGuid() }
            }
            };
            var user = new ApplicationUser { Id = userId };
            _mockSuggestionRepository.Setup(r => r.GetAllAttached())
                .Returns(new List<Suggestion> { suggestion }.AsQueryable().BuildMock());
            _mockLocationRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Location> { new Location { Id = Guid.NewGuid(), CityName = "Test City" } });

            // Act
            var result = await _suggestionService.GetSuggestionForEditAsync(suggestionId.ToString(), user);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNotNull(result.ViewModel);
            Assert.That(result.ViewModel.Title, Is.EqualTo("Test Suggestion"));
        }

        [Test]
        public async Task UpdateSuggestionAsync_ValidSuggestionAndUser_ReturnsSuccess()
        {
            // Arrange
            var suggestionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var suggestion = new Suggestion
            {
                Id = suggestionId,
                Title = "Old Title",
                UsersSuggestions = new List<ApplicationUserSuggestion>
            {
                new ApplicationUserSuggestion { ApplicationUserId = userId }
            },
                SuggestionsLocations = new List<SuggestionLocation>
            {
                new SuggestionLocation { LocationId = Guid.NewGuid() }
            }
            };
            _mockSuggestionRepository.Setup(r => r.GetAllAttached())
                .Returns(new List<Suggestion> { suggestion }.AsQueryable().BuildMock());
            var model = new SuggestionFormViewModel
            {
                Title = "New Title",
                CityId = Guid.NewGuid()
            };

            // Act
            var result = await _suggestionService.UpdateSuggestionAsync(suggestionId.ToString(), model, userId.ToString());

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            _mockSuggestionRepository.Verify(r => r.UpdateAsync(It.IsAny<Suggestion>()), Times.Once);
        }

        [Test]
        public async Task GetSuggestionForDeleteConfirmationAsync_ValidSuggestionAndUser_ReturnsViewModel()
        {
            // Arrange
            var suggestionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var suggestion = new Suggestion
            {
                Id = suggestionId,
                Title = "Test Suggestion",
                Category = "Test Category",
                Description = "Test Description",
                UsersSuggestions = new List<ApplicationUserSuggestion>
            {
                new ApplicationUserSuggestion { ApplicationUserId = userId }
            }
            };
            _mockSuggestionRepository.Setup(r => r.GetAllAttached())
                .Returns(new List<Suggestion> { suggestion }.AsQueryable().BuildMock());

            // Act
            var result = await _suggestionService.GetSuggestionForDeleteConfirmationAsync(suggestionId.ToString(), userId.ToString());

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNotNull(result.ViewModel);
            Assert.That(result.ViewModel.Title, Is.EqualTo("Test Suggestion"));
        }

        [Test]
        public async Task DeleteSuggestionAsync_ValidSuggestionAndUser_ReturnsSuccess()
        {
            // Arrange
            var suggestionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var suggestion = new Suggestion
            {
                Id = suggestionId,
                UsersSuggestions = new List<ApplicationUserSuggestion>
            {
                new ApplicationUserSuggestion { ApplicationUserId = userId }
            }
            };
            _mockSuggestionRepository.Setup(r => r.GetAllAttached())
                .Returns(new List<Suggestion> { suggestion }.AsQueryable().BuildMock());
            _mockCommentRepository.Setup(r => r.GetAllAttached())
                .Returns(new List<Comment>().AsQueryable().BuildMock());

            // Act
            var result = await _suggestionService.DeleteSuggestionAsync(suggestionId.ToString(), userId.ToString());

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            _mockSuggestionRepository.Verify(r => r.DeleteAsync(suggestionId), Times.Once);
        }

        [Test]
        public async Task GetCommentAsync_InvalidCommentId_ReturnsNull()
        {
            // Arrange
            var invalidCommentId = Guid.Empty;

            // Act
            var result = await _suggestionService.GetCommentAsync(invalidCommentId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task UpdateSuggestionAsync_InvalidSuggestionId_ReturnsError()
        {
            // Arrange
            var invalidSuggestionId = "invalid-id";
            var model = new SuggestionFormViewModel();
            var userId = Guid.NewGuid().ToString();

            // Act
            var result = await _suggestionService.UpdateSuggestionAsync(invalidSuggestionId, model, userId);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid suggestion ID."));
        }

        [Test]
        public async Task DeleteSuggestionAsync_UnauthorizedUser_ReturnsError()
        {
            // Arrange
            var suggestionId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var unauthorizedUserId = Guid.NewGuid();
            var suggestion = new Suggestion
            {
                Id = suggestionId,
                UsersSuggestions = new List<ApplicationUserSuggestion>
            {
                new ApplicationUserSuggestion { ApplicationUserId = ownerId }
            }
            };
            _mockSuggestionRepository.Setup(r => r.GetAllAttached())
                .Returns(new List<Suggestion> { suggestion }.AsQueryable().BuildMock());

            // Act
            var result = await _suggestionService.DeleteSuggestionAsync(suggestionId.ToString(), unauthorizedUserId.ToString());

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.That(result.ErrorMessage, Is.EqualTo("You are not authorized to delete this suggestion."));
        }
    }
}
