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

        public async Task<IActionResult> Delete(int id)
        {
            //Remove sentiment from database table
            bool success = _dataSource.RemoveSentiment(id);
            System.Diagnostics.Debug.WriteLine($"Delete succeeded: {success}");

            //Then, we want to go back to Index
            return RedirectToAction("Maintenance");
        }

        public IActionResult Edit(int id)
        {
            SentimentModel sentiment = _dataSource.GetSentiment(id);

            return View("Edit", sentiment);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SentimentModel s)
        {
            //Edit the sentiment using the new information
            bool success = _dataSource.EditSentiment(s.Id, s.SentimentResult);
            System.Diagnostics.Debug.WriteLine($"Edit succeeded: {success}");

            return RedirectToAction("Maintenance");
        }
    }
}