using AdminGUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AdminGUI.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}