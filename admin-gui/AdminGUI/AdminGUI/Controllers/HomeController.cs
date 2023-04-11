using AdminGUI.Connectors;
using AdminGUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AdminGUI.Controllers
{
    public class HomeController : Controller
    {
        private IDataSource _dataSource;

        public HomeController(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Maintenance()
        {
            //Query the database for the sentiments
            List<SentimentModel>? sentiments = _dataSource.GetSentiments();

            //If there are no sentiments, set the List to null
            if (sentiments.Count == 0)
                sentiments = null;

            //Return the maintenance page view, showing the queried sentiments in the list view
            return View("Maintenance", sentiments);
        }
    }
}