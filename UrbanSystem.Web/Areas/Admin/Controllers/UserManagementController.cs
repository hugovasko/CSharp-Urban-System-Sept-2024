using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrbanSystem.Data.Models;
using static UrbanSystem.Common.ApplicationConstants;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.Controllers;

namespace UrbanSystem.Web.Areas.Admin.Controllers
{
    [Area(AdminRoleName)]
    [Authorize(Roles = AdminRoleName)]
    public class UserManagementController : BaseController
    {
        private readonly IUserService _userService;

        public UserManagementController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            Guid userGuid = Guid.Empty;

            if (!IsGuidIdValid(userId, ref userGuid))
            {
                return RedirectToAction(nameof(Index));
            }

            bool userExists = await _userService.UserExistsByIdAsync(userGuid);
            if (!userExists)
            {
                return RedirectToAction(nameof(Index));
            }

            bool assignResult = await _userService.AssignUserToRoleAsync(userGuid, role);
            if (!assignResult)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            Guid userGuid = Guid.Empty;

            if (!IsGuidIdValid(userId, ref userGuid))
            {
                return RedirectToAction(nameof(Index));
            }

            bool userExists = await _userService.UserExistsByIdAsync(userGuid);
            if (!userExists)
            {
                return RedirectToAction(nameof(Index));
            }

            bool removeResult = await _userService.RemoveUserFromRoleAsync(userGuid, role);
            if (!removeResult)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            Guid userGuid = Guid.Empty;

            if (!IsGuidIdValid(userId, ref userGuid))
            {
                return RedirectToAction(nameof(Index));
            }

            bool userExists = await _userService.UserExistsByIdAsync(userGuid);
            if (!userExists)
            {
                return RedirectToAction(nameof(Index));
            }

            bool removeResult = await _userService.DeleteUserAsync(userGuid);
            if (!removeResult)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}