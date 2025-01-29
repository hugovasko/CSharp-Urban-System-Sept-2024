using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static UrbanSystem.Common.ApplicationConstants;

namespace UrbanSystem.Web.Areas.Admin.Controllers
{
    [Area(AdminRoleName)]
    [Authorize(Roles = AdminRoleName)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
