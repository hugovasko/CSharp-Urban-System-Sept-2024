using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.ViewModels.Locations;
using static UrbanSystem.Common.ApplicationConstants;
using static UrbanSystem.Common.ValidationStrings.LocationControllerMessages;

namespace UrbanSystem.Web.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class LocationController : BaseController
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationController> _logger;

        public LocationController(ILocationService locationService, ILogger<LocationController> logger) : base()
        {
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All(int? page)
        {
            try
            {
                int pageNumber = page ?? 1;
                int pageSize = 9;
                var locations = await _locationService.GetAllOrderedByNameAsync(pageNumber, pageSize);
                return View(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, FetchAllLocationsError);
                return StatusCode(500, GeneralProcessingError);
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new LocationFormViewModel());
        }

        [HttpPost]
        [Authorize(Roles = AdminRoleName)]
        public async Task<IActionResult> Add([FromForm] LocationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _locationService.AddLocationAsync(model);
                _logger.LogInformation(LocationAddedLog);
                return RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, AddLocationError);
                ModelState.AddModelError(string.Empty, AddLocationRetryError);
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(InvalidLocationIdError);
            }

            try
            {
                var details = await _locationService.GetLocationDetailsByIdAsync(id);

                if (details == null)
                {
                    _logger.LogWarning(LocationNotFoundLog, id);
                    return NotFound(LocationNotFoundError);
                }

                return View(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, FetchLocationDetailsError, id);
                return StatusCode(500, GeneralProcessingError);
            }
        }
    }
}
