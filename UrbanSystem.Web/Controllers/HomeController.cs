using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static UrbanSystem.Common.ValidationStrings.Home;
using UrbanSystem.Web.ViewModels;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Data.Models;
using UrbanSystem.Web.ViewModels.Projects;

namespace UrbanSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Project, Guid> _projectRepository;

        public HomeController(IRepository<Project, Guid> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public IActionResult Index()
        {
            ViewData[TitleKey] = TitleValue;
            ViewData[MessageKey] = WelcomeMessage;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AboutUs()
        {
            return View();
        }
    }
}