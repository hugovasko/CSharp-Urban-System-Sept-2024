using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.ViewModels.Admin.UserManagement;

namespace UrbanSystem.Services.Data
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IRepository<Meeting, Guid> _meetingRepository;
        private readonly IRepository<Comment, Guid> _commentRepository;
        private readonly IRepository<Suggestion, Guid> _suggestionRepository;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IRepository<Meeting, Guid> meetingRepository, IRepository<Comment, Guid> commentRepository, IRepository<Suggestion, Guid> suggestionRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _meetingRepository = meetingRepository;
            _commentRepository = commentRepository;
            _suggestionRepository = suggestionRepository;
        }

        public async Task<bool> AssignUserToRoleAsync(Guid userId, string roleName)
        {
            ApplicationUser? user = await _userManager
                .FindByIdAsync(userId.ToString());
            bool roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (user == null || !roleExists)
            {
                return false;
            }

            bool alreadyInRole = await _userManager.IsInRoleAsync(user, roleName);

            if (!alreadyInRole)
            {
                IdentityResult? result = await _userManager.AddToRoleAsync(user, roleName);

                if (!result.Succeeded)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            await _commentRepository.DeleteAsync(c => c.UserId == userId);

            var userSuggestions = await _suggestionRepository.GetAllAsync(
                s => s.UsersSuggestions.Any(aus => aus.ApplicationUserId == userId));

            foreach (var suggestion in userSuggestions)
            {
                await _suggestionRepository.DeleteAsync(suggestion.Id);
            }

            var meetingsAttending = await _meetingRepository.GetAllAsync();
            foreach (var meeting in meetingsAttending)
            {
                meeting.Attendees.Remove(user);
                await _meetingRepository.UpdateAsync(meeting);
            }

            await _meetingRepository.DeleteAsync(m => m.OrganizerId == userId);

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded;
        }

        public async Task<IEnumerable<UsersViewModel>> GetAllUsersAsync()
        {
            ApplicationUser[] allUsers = _userManager.Users
                .ToArray();
            ICollection<UsersViewModel> allUsersViewModel = new List<UsersViewModel>();

            foreach (ApplicationUser user in allUsers)
            {
                IEnumerable<string> roles = await _userManager.GetRolesAsync(user);

                allUsersViewModel.Add(new UsersViewModel()
                {
                    Id = user.Id.ToString(),
                    Email = user.Email,
                    Roles = roles
                });
            }

            return allUsersViewModel;
        }

        public async Task<bool> RemoveUserFromRoleAsync(Guid userId, string roleName)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());

            bool roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (user == null || !roleExists)
            {
                return false;
            }

            bool alreadyInRole = await _userManager.IsInRoleAsync(user, roleName);

            if (alreadyInRole)
            {
                IdentityResult? result = await _userManager
                    .RemoveFromRoleAsync(user, roleName);

                if (!result.Succeeded)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> UserExistsByIdAsync(Guid userId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());

            return user != null;
        }
    }
}