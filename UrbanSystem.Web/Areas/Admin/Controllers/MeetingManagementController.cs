using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static UrbanSystem.Common.ApplicationConstants;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.Controllers;

namespace UrbanSystem.Web.Areas.Admin.Controllers
{
    [Area(AdminRoleName)]
    [Authorize(Roles = AdminRoleName)]
    public class MeetingManagementController : BaseController
    {
        private readonly IMeetingManagementService _meetingManagementService;

        public MeetingManagementController(IMeetingManagementService meetingManagementService, IBaseService baseService)
            : base(baseService)
        {
            _meetingManagementService = meetingManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var meetings = await _meetingManagementService.GetAllMeetingsAsync();
            return View(meetings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            Guid meetingGuid = Guid.Empty;

            if (!IsGuidIdValid(id, ref meetingGuid))
            {
                return RedirectToAction(nameof(Index));
            }

            bool deleteResult = await _meetingManagementService.DeleteMeetingAsync(meetingGuid);
            if (!deleteResult)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}