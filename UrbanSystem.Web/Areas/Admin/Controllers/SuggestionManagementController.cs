using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static UrbanSystem.Common.ApplicationConstants;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Web.Controllers;
using UrbanSystem.Web.ViewModels.Suggestions;

namespace UrbanSystem.Web.Areas.Admin.Controllers
{
    [Area(AdminRoleName)]
    [Authorize(Roles = AdminRoleName)]
    public class SuggestionManagementController : BaseController
    {
        private readonly ISuggestionManagementService _suggestionManagementService;

        public SuggestionManagementController(ISuggestionManagementService suggestionManagementService)
        {
            _suggestionManagementService = suggestionManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var suggestions = await _suggestionManagementService.GetAllSuggestionsAsync();
            return View(suggestions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            Guid suggestionGuid = Guid.Empty;

            if (!IsGuidIdValid(id, ref suggestionGuid))
            {
                return RedirectToAction(nameof(Index));
            }

            bool deleteResult = await _suggestionManagementService.DeleteSuggestionAsync(suggestionGuid);
            if (!deleteResult)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(string id, string status)
        {
            Guid suggestionGuid = Guid.Empty;

            if (!IsGuidIdValid(id, ref suggestionGuid))
            {
                return RedirectToAction(nameof(Index));
            }

            bool updateResult = await _suggestionManagementService.UpdateSuggestionStatusAsync(suggestionGuid, status);
            if (!updateResult)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePriority(string id, string priority)
        {
            Guid suggestionGuid = Guid.Empty;

            if (!IsGuidIdValid(id, ref suggestionGuid))
            {
                return RedirectToAction(nameof(Index));
            }

            bool updateResult = await _suggestionManagementService.UpdateSuggestionPriorityAsync(suggestionGuid, priority);
            if (!updateResult)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}