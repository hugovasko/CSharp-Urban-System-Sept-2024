using Moq;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data;
using MockQueryable.Moq;

namespace ServiceTests
{
    [TestFixture]
    public class MySuggestionServiceTests
    {
        private Mock<IRepository<ApplicationUserSuggestion, object>> _mockUserSuggestionRepository;
        private MySuggestionService _mySuggestionService;

        [SetUp]
        public void Setup()
        {
            _mockUserSuggestionRepository = new Mock<IRepository<ApplicationUserSuggestion, object>>();
            _mySuggestionService = new MySuggestionService(_mockUserSuggestionRepository.Object);
        }

        [Test]
        public async Task GetAllSuggestionsForLoggedInUser_ReturnsCorrectViewModel_WhenSuggestionsExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var location1 = new Location { Id = Guid.NewGuid(), CityName = "New York" };
            var location2 = new Location { Id = Guid.NewGuid(), CityName = "Los Angeles" };
            var suggestionId = Guid.NewGuid();

            var testSuggestions = new List<ApplicationUserSuggestion>
            {
                new ApplicationUserSuggestion
                {
                    ApplicationUserId = Guid.Parse(userId),
                    Suggestion = new Suggestion
                    {
                        Id = suggestionId,
                        Title = "Test Suggestion",
                        Category = "General",
                        UploadedOn = new DateTime(2024, 1, 1, 10, 0, 0),
                        AttachmentUrl = "http://example.com/attachment",
                        SuggestionsLocations = new List<SuggestionLocation>
                        {
                            new SuggestionLocation { Location = location1 },
                            new SuggestionLocation { Location = location2 }
                        }
                    }
                }
            };

            _mockUserSuggestionRepository.Setup(repo => repo.GetAllAttached())
                .Returns(testSuggestions.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _mySuggestionService.GetAllSuggestionsForLoggedInUser(userId);

            // Assert
            Assert.IsNotNull(result);
            var resultList = result.ToList();
            Assert.That(resultList.Count, Is.EqualTo(1));
            var suggestionViewModel = resultList.First();

            Assert.That(suggestionViewModel.Id, Is.EqualTo(suggestionId.ToString()));
            Assert.That(suggestionViewModel.Title, Is.EqualTo("Test Suggestion"));
            Assert.That(suggestionViewModel.Category, Is.EqualTo("General"));
            Assert.That(suggestionViewModel.UploadedOn, Is.EqualTo("01/01/2024 10:00"));
            Assert.That(suggestionViewModel.AttachmentUrl, Is.EqualTo("http://example.com/attachment"));
            Assert.That(suggestionViewModel.LocationNames.Count(), Is.EqualTo(2));
            Assert.IsTrue(suggestionViewModel.LocationNames.Any(l => l.Text == "New York"));
            Assert.IsTrue(suggestionViewModel.LocationNames.Any(l => l.Text == "Los Angeles"));
        }

        [Test]
        public async Task GetAllSuggestionsForLoggedInUser_ReturnsEmptyList_WhenNoSuggestionsExistForUser()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            _mockUserSuggestionRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<ApplicationUserSuggestion>().AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _mySuggestionService.GetAllSuggestionsForLoggedInUser(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetAllSuggestionsForLoggedInUser_OrdersByUploadedOn()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var suggestion1 = new ApplicationUserSuggestion
            {
                ApplicationUserId = Guid.Parse(userId),
                Suggestion = new Suggestion
                {
                    Id = Guid.NewGuid(),
                    Title = "Suggestion 1",
                    Category = "General",
                    UploadedOn = new DateTime(2024, 1, 1, 10, 0, 0)
                }
            };
            var suggestion2 = new ApplicationUserSuggestion
            {
                ApplicationUserId = Guid.Parse(userId),
                Suggestion = new Suggestion
                {
                    Id = Guid.NewGuid(),
                    Title = "Suggestion 2",
                    Category = "Specific",
                    UploadedOn = new DateTime(2024, 1, 2, 10, 0, 0)
                }
            };

            var testSuggestions = new List<ApplicationUserSuggestion> { suggestion2, suggestion1 };

            _mockUserSuggestionRepository.Setup(repo => repo.GetAllAttached())
                .Returns(testSuggestions.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _mySuggestionService.GetAllSuggestionsForLoggedInUser(userId);

            // Assert
            var resultList = result.ToList();
            Assert.That(resultList.Count, Is.EqualTo(2));
            Assert.That(resultList[0].Title, Is.EqualTo("Suggestion 1"));
            Assert.That(resultList[1].Title, Is.EqualTo("Suggestion 2"));
        }

        [Test]
        public async Task GetAllSuggestionsForLoggedInUser_HandlesCaseInsensitiveUserId()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString().ToUpper();
            var suggestion = new ApplicationUserSuggestion
            {
                ApplicationUserId = Guid.Parse(userId.ToLower()),
                Suggestion = new Suggestion
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Suggestion",
                    UploadedOn = DateTime.UtcNow
                }
            };

            _mockUserSuggestionRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<ApplicationUserSuggestion> { suggestion }.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _mySuggestionService.GetAllSuggestionsForLoggedInUser(userId);

            // Assert
            Assert.IsNotEmpty(result);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Test Suggestion"));
        }

        [Test]
        public async Task GetAllSuggestionsForLoggedInUser_HandlesNullValues()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var suggestion = new ApplicationUserSuggestion
            {
                ApplicationUserId = Guid.Parse(userId),
                Suggestion = new Suggestion
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Suggestion",
                    UploadedOn = DateTime.UtcNow,
                    AttachmentUrl = null,
                    SuggestionsLocations = null!
                }
            };

            _mockUserSuggestionRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<ApplicationUserSuggestion> { suggestion }.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _mySuggestionService.GetAllSuggestionsForLoggedInUser(userId);

            // Assert
            Assert.IsNotEmpty(result);
            var viewModel = result.First();
            Assert.IsNull(viewModel.AttachmentUrl);
            Assert.IsEmpty(viewModel.LocationNames);
        }

        [Test]
        public async Task GetAllSuggestionsForLoggedInUser_FiltersSuggestionsForSpecificUser()
        {
            // Arrange
            var userId1 = Guid.NewGuid().ToString();
            var userId2 = Guid.NewGuid().ToString();
            var suggestion1 = new ApplicationUserSuggestion
            {
                ApplicationUserId = Guid.Parse(userId1),
                Suggestion = new Suggestion { Id = Guid.NewGuid(), Title = "User 1 Suggestion", UploadedOn = DateTime.UtcNow }
            };
            var suggestion2 = new ApplicationUserSuggestion
            {
                ApplicationUserId = Guid.Parse(userId2),
                Suggestion = new Suggestion { Id = Guid.NewGuid(), Title = "User 2 Suggestion", UploadedOn = DateTime.UtcNow }
            };

            _mockUserSuggestionRepository.Setup(repo => repo.GetAllAttached())
                .Returns(new List<ApplicationUserSuggestion> { suggestion1, suggestion2 }.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _mySuggestionService.GetAllSuggestionsForLoggedInUser(userId1);

            // Assert
            Assert.IsNotEmpty(result);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("User 1 Suggestion"));
        }
    }
}