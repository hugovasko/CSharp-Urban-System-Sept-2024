using Microsoft.AspNetCore.Mvc;
using UrbanSystem.Web.ViewModels.Locations;
using UrbanSystem.Services.Data.Contracts;

namespace UrbanSystem.Web.Controllers
{
    public class BaseController : Controller
	{
        public IBaseService _baseService { get; set; }

        public BaseController(IBaseService? baseService = null)
        {
            _baseService = baseService!;
        }

        protected bool IsGuidIdValid(string? id, ref Guid locationGuid)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return false;
			}

			return Guid.TryParse(id.ToLower(), out locationGuid);
        }

        protected async Task<IEnumerable<CityOption>> CityList()
        {
            var cities = await _baseService.GetCitiesAsync();
            return cities;
        }
    }
}
