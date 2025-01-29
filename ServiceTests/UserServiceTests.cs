using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data;
using UrbanSystem.Web.ViewModels.Admin.UserManagement;

namespace UrbanSystem.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<RoleManager<IdentityRole<Guid>>> _roleManagerMock;
        private Mock<IRepository<Meeting, Guid>> _meetingRepositoryMock;
        private Mock<IRepository<Comment, Guid>> _commentRepositoryMock;
        private Mock<IRepository<Suggestion, Guid>> _suggestionRepositoryMock;

        private UserService _userService;

        [SetUp]
        public void SetUp()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole<Guid>>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(roleStoreMock.Object, null, null, null, null);

            _meetingRepositoryMock = new Mock<IRepository<Meeting, Guid>>();
            _commentRepositoryMock = new Mock<IRepository<Comment, Guid>>();
            _suggestionRepositoryMock = new Mock<IRepository<Suggestion, Guid>>();

            _userService = new UserService(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _meetingRepositoryMock.Object,
                _commentRepositoryMock.Object,
                _suggestionRepositoryMock.Object
            );
        }

        [Test]
        public async Task AssignUserToRoleAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleName = "Admin";

            _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser)null);

            _roleManagerMock.Setup(m => m.RoleExistsAsync(roleName))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.AssignUserToRoleAsync(userId, roleName);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task AssignUserToRoleAsync_ShouldReturnTrue_WhenUserAssignedToRole()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleName = "Admin";
            var user = new ApplicationUser { Id = userId };

            _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _roleManagerMock.Setup(m => m.RoleExistsAsync(roleName))
                .ReturnsAsync(true);

            _userManagerMock.Setup(m => m.IsInRoleAsync(user, roleName))
                .ReturnsAsync(false);

            _userManagerMock.Setup(m => m.AddToRoleAsync(user, roleName))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.AssignUserToRoleAsync(userId, roleName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldDeleteUserDependenciesAndReturnTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId };

            _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _commentRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Expression<Func<Comment, bool>>>()))
                    .ReturnsAsync(true);

            _suggestionRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Suggestion, bool>>>()))
                .ReturnsAsync(new List<Suggestion>());

            _meetingRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Meeting>());


            _userManagerMock.Setup(m => m.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.IsTrue(result);
            _commentRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Expression<Func<Comment, bool>>>()), Times.Once);
            _userManagerMock.Verify(m => m.DeleteAsync(user), Times.Once);
        }

        [Test]
        public async Task GetAllUsersAsync_ShouldReturnListOfUsersWithRoles()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = Guid.NewGuid(), Email = "user1@example.com" },
                new ApplicationUser { Id = Guid.NewGuid(), Email = "user2@example.com" }
            };

            _userManagerMock.Setup(m => m.Users)
                .Returns(users.AsQueryable());

            _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "Admin" });

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.That(result.All(u => u.Roles.Contains("Admin")));
        }

        [Test]
        public async Task UserExistsByIdAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId };

            _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.UserExistsByIdAsync(userId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task UserExistsByIdAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _userService.UserExistsByIdAsync(userId);

            // Assert
            Assert.IsFalse(result);
        }
    }
}