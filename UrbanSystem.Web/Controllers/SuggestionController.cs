using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrbanSystem.Data.Models;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.ViewModels.Suggestions;
using static UrbanSystem.Common.ValidationStrings.SuggestionControllerMessages;

namespace UrbanSystem.Web.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class SuggestionController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISuggestionService _suggestionService;
        private readonly ILogger<SuggestionController> _logger;

        public SuggestionController(
            IBaseService baseService,
            UserManager<ApplicationUser> userManager,
            ISuggestionService suggestionService,
            ILogger<SuggestionController> logger) : base(baseService)
        {
            _userManager = userManager;
            _suggestionService = suggestionService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All(string searchQuery, string sortBy, bool ascending = true, int? page = 1)
        {
            int pageSize = 10; // You can adjust this value as needed
            var suggestions = await _suggestionService.GetAllSuggestionsAsync(page ?? 1, pageSize, searchQuery, sortBy, ascending);

            ViewData["SearchQuery"] = searchQuery;
            ViewData["SortBy"] = sortBy;
            ViewData["Ascending"] = ascending;

            return View(suggestions);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var viewModel = await _suggestionService.GetSuggestionFormViewModelAsync();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] SuggestionFormViewModel suggestionModel)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _suggestionService.AddSuggestionAsync(suggestionModel, userId!);

            if (!result.IsSuccessful)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(result.ViewModel);
            }

            _logger.LogInformation(AddSuggestionLog, userId);
            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(InvalidSuggestionId);
            }

            var userId = _userManager.GetUserId(User);
            var result = await _suggestionService.GetSuggestionDetailsAsync(id, userId!);

            if (!result.IsSuccessful)
            {
                _logger.LogWarning($"Failed to retrieve suggestion details for ID: {id}. Error: {result.ErrorMessage}");
                TempData["ErrorMessage"] = SuggestionNotFoundError;
                return RedirectToAction(nameof(All));
            }

            return View(result.Suggestion);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string suggestionId, [FromForm] string content)
        {
            if (string.IsNullOrEmpty(suggestionId) || string.IsNullOrEmpty(content))
            {
                return BadRequest(InvalidSuggestionOrCommentContent);
            }

            var userId = _userManager.GetUserId(User);
            var result = await _suggestionService.AddCommentAsync(suggestionId, content, userId!);

            if (!result.IsSuccessful)
            {
                _logger.LogWarning($"Failed to add comment to suggestion {suggestionId}. Error: {result.ErrorMessage}");
                return BadRequest(AddSuggestionError);
            }

            return RedirectToAction(nameof(Details), new { id = suggestionId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(InvalidSuggestionId);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var result = await _suggestionService.GetSuggestionForEditAsync(id, currentUser!);

            if (!result.IsSuccessful)
            {
                _logger.LogWarning(RetrieveSuggestionForEditLog, id, currentUser!.Id, result.ErrorMessage);
                TempData["ErrorMessage"] = RetrieveSuggestionForEditError;
                return RedirectToAction(nameof(All));
            }

            return View(result.ViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, [FromForm] SuggestionFormViewModel model)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(InvalidSuggestionId);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var result = await _suggestionService.UpdateSuggestionAsync(id, model, currentUser!.Id.ToString());

            if (!result.IsSuccessful)
            {
                _logger.LogWarning(UpdateSuggestionError!, id, currentUser.Id, result.ErrorMessage);
                TempData["ErrorMessage"] = UpdateSuggestionError;
                return View(model);
            }

            _logger.LogInformation(UpdateSuggestionLog, currentUser.Id, id);
            TempData["SuccessMessage"] = SuggestionUpdateSuccess;
            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(InvalidSuggestionId);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var result = await _suggestionService.GetSuggestionForDeleteConfirmationAsync(id, currentUser!.Id.ToString());

            if (!result.IsSuccessful)
            {
                _logger.LogWarning(RetrieveSuggestionForDeleteLog, id, currentUser.Id, result.ErrorMessage);
                TempData["ErrorMessage"] = RetrieveSuggestionForDeleteError;
                return RedirectToAction(nameof(All));
            }

            return View(result.ViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(InvalidSuggestionId);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var result = await _suggestionService.DeleteSuggestionAsync(id, currentUser!.Id.ToString());

            if (!result.IsSuccessful)
            {
                _logger.LogWarning(DeleteSuggestionLog!, id, currentUser.Id, result.ErrorMessage);
                TempData["ErrorMessage"] = DeleteSuggestionError;
                return RedirectToAction(nameof(Details), new { id = id });
            }

            _logger.LogInformation(DeleteSuggestionLog, currentUser.Id, id);
            TempData["SuccessMessage"] = SuggestionDeleteSuccess;
            return RedirectToAction(nameof(All));
        }
    }
}
