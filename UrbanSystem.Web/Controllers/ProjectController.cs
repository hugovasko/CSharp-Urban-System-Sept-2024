using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Services.Interfaces;
using UrbanSystem.Web.ViewModels.Projects;
using static UrbanSystem.Common.ApplicationConstants;
using static UrbanSystem.Common.ValidationStrings.ProjectControllerMessages;

namespace UrbanSystem.Web.Controllers
{
    [Authorize(Roles = AdminRoleName)]
    [AutoValidateAntiforgeryToken]
    public class ProjectController : BaseController
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(
            IBaseService baseService,
            IProjectService projectService,
            ILogger<ProjectController> logger) : base(baseService)
        {
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _baseService = baseService ?? throw new ArgumentNullException(nameof(baseService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All()
        {
            try
            {
                var projects = await _projectService.GetAllProjectsAsync();
                return View(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, FetchAllProjectsError);
                return StatusCode(500, ProcessingRequestError);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(InvalidProjectId);
            }

            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);

                if (project == null)
                {
                    _logger.LogWarning(ProjectNotFoundError!, id);
                    return NotFound(ProjectNotFoundError);
                }

                return View(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(FetchProjectDetailsError, id));
                return StatusCode(500, ProcessingRequestError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            try
            {
                var cities = await CityList();

                var viewModel = new ProjectFormViewModel
                {
                    Cities = cities
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, AddProjectFormError);
                return StatusCode(500, ProcessingRequestError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] ProjectFormViewModel model)
        {
            try
            {
                await _projectService.AddProjectAsync(model);
                _logger.LogInformation(NewProjectAdded);
                return RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, AddProjectError);
                ModelState.AddModelError(string.Empty, AddProjectRetryError);
                model.Cities = await CityList();
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(InvalidProjectId);
            }

            try
            {
                var isDeleted = await _projectService.DeleteProjectAsync(id);

                if (!isDeleted)
                {
                    _logger.LogWarning(DeleteNonExistentProjectWarning, id);
                    return NotFound(DeleteNonExistentProjectWarning);
                }

                _logger.LogInformation(ProjectDeletedSuccessfully, id);
                return RedirectToAction(nameof(All));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, string.Format(InvalidOperationWhileDeletingProject, id));
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(DeleteProjectError, id));
                TempData["ErrorMessage"] = DeleteProjectUnexpectedError;
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}