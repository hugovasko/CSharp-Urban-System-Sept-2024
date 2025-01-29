using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static UrbanSystem.Common.ApplicationConstants;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.Controllers;
using UrbanSystem.Web.ViewModels.Projects;

namespace UrbanSystem.Web.Areas.Admin.Controllers
{
    [Area(AdminRoleName)]
    [Authorize(Roles = AdminRoleName)]
    public class ProjectManagementController : BaseController
    {
        private readonly IProjectManagementService _projectManagementService;

        public ProjectManagementController(IProjectManagementService projectManagementService, IBaseService baseService)
            : base(baseService)
        {
            _projectManagementService = projectManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var projects = await _projectManagementService.GetAllProjectsAsync();
            return View(projects);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            Guid projectGuid = Guid.Empty;

            if (!IsGuidIdValid(id, ref projectGuid))
            {
                return RedirectToAction(nameof(Index));
            }

            var project = await _projectManagementService.GetProjectByIdAsync(projectGuid);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            Guid projectGuid = Guid.Empty;

            if (!IsGuidIdValid(id, ref projectGuid))
            {
                return RedirectToAction(nameof(Index));
            }

            bool deleteResult = await _projectManagementService.DeleteProjectAsync(projectGuid);
            if (!deleteResult)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCompletion(string id, bool IsCompleted)
        {
            Guid projectGuid = Guid.Empty;

            if (!IsGuidIdValid(id, ref projectGuid))
            {
                return RedirectToAction(nameof(Index));
            }

            bool toggleResult = await _projectManagementService.UpdateProjectCompletionAsync(projectGuid, IsCompleted);
            if (!toggleResult)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}