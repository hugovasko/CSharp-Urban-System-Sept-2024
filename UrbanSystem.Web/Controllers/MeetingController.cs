using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static UrbanSystem.Common.ValidationStrings.Meeting;
using UrbanSystem.Data.Models;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.ViewModels.Meetings;

namespace UrbanSystem.Web.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class MeetingController : BaseController
    {
        private readonly IMeetingService _meetingService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<MeetingController> _logger;

        public MeetingController(
            IBaseService baseService,
            IMeetingService meetingService,
            UserManager<ApplicationUser> userManager,
            ILogger<MeetingController> logger) : base(baseService)
        {
            _meetingService = meetingService ?? throw new ArgumentNullException(nameof(meetingService));
            _baseService = baseService ?? throw new ArgumentNullException(nameof(baseService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All()
        {
            try
            {
                var meetings = await _meetingService.GetAllMeetingsAsync();
                var currentUser = await _userManager.GetUserAsync(User);
                foreach (var meeting in meetings)
                {
                    meeting.IsCurrentUserOrganizer = currentUser != null && meeting.OrganizerId == currentUser.Id;
                }
                return View(meetings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorFetchingAllMeetings);
                return StatusCode(500, ErrorProcessingRequest);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            try
            {
                var viewModel = await _meetingService.GetMeetingFormViewModelAsync();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorPreparingAddMeetingForm);
                return StatusCode(500, ErrorProcessingRequest);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] MeetingFormViewModel meetingForm)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    meetingForm = await _meetingService.GetMeetingFormViewModelAsync(meetingForm);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ErrorRepopulatingAddMeetingForm);
                }
                return View(meetingForm);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                await _meetingService.CreateMeetingAsync(meetingForm, currentUser.UserName!);
                _logger.LogInformation(NewMeetingCreatedByUser, currentUser.Id);
                return RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorCreatingNewMeeting);
                ModelState.AddModelError(string.Empty, ErrorProcessingRequest);
                meetingForm = await _meetingService.GetMeetingFormViewModelAsync(meetingForm);
                return View(meetingForm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(InvalidMeetingId);
            }

            try
            {
                var meeting = await _meetingService.GetMeetingForEditAsync(id);
                if (meeting == null)
                {
                    return NotFound();
                }

                meeting.Cities = await CityList();

                return View(meeting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorPreparingEditMeeting, id);
                return StatusCode(500, ErrorProcessingRequest);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, [FromForm] MeetingEditViewModel meetingEdit)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(InvalidMeetingId);
            }

            if (!ModelState.IsValid)
            {
                meetingEdit.Cities = await CityList();
                return View(meetingEdit);
            }

            try
            {
                var meeting = await _meetingService.GetMeetingByIdAsync(id);
                if (meeting == null)
                {
                    return NotFound();
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || meeting.OrganizerId != currentUser.Id)
                {
                    _logger.LogWarning(UnauthorizedEditAttempt, id, currentUser?.Id);
                    return Forbid();
                }

                meetingEdit.Duration = double.Parse(meetingEdit.Duration.ToString());

                var meetingForm = new MeetingEditViewModel
                {
                    Title = meetingEdit.Title,
                    Description = meetingEdit.Description,
                    ScheduledDate = meetingEdit.ScheduledDate,
                    Duration = meetingEdit.Duration,
                    LocationId = meetingEdit.LocationId
                };

                await _meetingService.UpdateMeetingAsync(id, meetingForm);
                _logger.LogInformation(MeetingUpdatedByUser, id, currentUser.Id);

                return RedirectToAction(nameof(All));
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorUpdatingMeeting, id);
                ModelState.AddModelError(string.Empty, ErrorProcessingRequest);
                meetingEdit.Cities = await CityList();
                return View(meetingEdit);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(InvalidMeetingId);
            }

            try
            {
                var meeting = await _meetingService.GetMeetingByIdAsync(id);
                if (meeting == null)
                {
                    return NotFound();
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || meeting.OrganizerId != currentUser.Id)
                {
                    _logger.LogWarning(UnauthorizedDeleteAttempt, id, currentUser?.Id);
                    return Forbid();
                }

                return View(meeting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorPreparingDeleteMeeting, id);
                return StatusCode(500, ErrorProcessingRequest);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(InvalidMeetingId);
            }

            try
            {
                var meeting = await _meetingService.GetMeetingByIdAsync(id);
                if (meeting == null)
                {
                    return NotFound();
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || meeting.OrganizerId != currentUser.Id)
                {
                    _logger.LogWarning(UnauthorizedDeleteAttempt, id, currentUser?.Id);
                    return Forbid();
                }

                await _meetingService.DeleteMeetingAsync(id);
                _logger.LogInformation(ErrorDeletingMeeting!, id, currentUser.Id);
                return RedirectToAction(nameof(All));
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorDeletingMeeting, id);
                TempData["ErrorMessage"] = ErrorDeletingMeetingMessage;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(InvalidMeetingId);
            }

            try
            {
                var meeting = await _meetingService.GetMeetingByIdAsync(id);
                if (meeting == null)
                {
                    return NotFound();
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    meeting.IsCurrentUserAttending = meeting.Attendees.Contains(currentUser.UserName);
                }

                meeting.IsCurrentUserOrganizer = currentUser != null && meeting.OrganizerId == currentUser?.Id;

                return View(meeting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorFetchingMeetingDetails, id);
                return StatusCode(500, ErrorProcessingRequest);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Attend(Guid id)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Unauthorized();
                }

                await _meetingService.AttendMeetingAsync(currentUser.UserName!, id);
                _logger.LogInformation(UserRegisteredForMeeting, currentUser.Id, id);
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while attending meeting {MeetingId}"!, id);
                return StatusCode(500, ErrorProcessingRequest);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelAttendance(Guid id)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Unauthorized();
                }

                await _meetingService.CancelAttendanceAsync(currentUser.UserName!, id);
                _logger.LogInformation(UserCancelledAttendanceForMeeting, currentUser.Id, id);
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorCancelingAttendance!, id);
                return StatusCode(500, ErrorProcessingRequest);
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyMeetings()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                var viewModel = await _meetingService.GetUserAttendedMeetingsAsync(currentUser.UserName!);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching attended meetings for user {UserId}", currentUser.Id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}