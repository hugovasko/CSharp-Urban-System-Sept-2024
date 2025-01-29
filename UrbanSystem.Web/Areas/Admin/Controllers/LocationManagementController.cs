using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrbanSystem.Data.Models;
using static UrbanSystem.Common.ApplicationConstants;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.Controllers;
using UrbanSystem.Services.Interfaces;

namespace UrbanSystem.Web.Areas.Admin.Controllers
{
    [Area(AdminRoleName)]
    [Authorize(Roles = AdminRoleName)]
    public class LocationManagementController : BaseController
    {
        private readonly ILocationManagementService _locationService;
        private readonly IMeetingService _meetingService;
        private readonly IProjectService _projectService;

        public LocationManagementController(
            ILocationManagementService locationService,
            IMeetingService meetingService,
            IProjectService projectService)
        {
            _locationService = locationService;
            _meetingService = meetingService;
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return View(locations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            Guid locationGuid = Guid.Empty;

            if (!IsGuidIdValid(id, ref locationGuid))
            {
                return RedirectToAction(nameof(Index));
            }

            var location = await _locationService.GetLocationByIdAsync(locationGuid);
            if (location == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await _locationService.DeleteSuggestionsByLocationIdAsync(locationGuid);

            await _locationService.DeleteMeetingsByLocationIdAsync(locationGuid);

            await _locationService.DeleteProjectsByLocationIdAsync(locationGuid);

            await _locationService.DeleteLocationAsync(locationGuid);

            return RedirectToAction(nameof(Index));
        }
    }
}