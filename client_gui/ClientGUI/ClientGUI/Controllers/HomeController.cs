using ClientGUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ClientGUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private string SENTIMENT_SOURCE = @"https://localhost:1234/analyze";
        private string DATABASE_SOURCE = @"https://localhost:1234/";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<SentimentModel> sentiments = new List<SentimentModel>();

            //Need to have database set up - for now, I'll just hardcode some in
            /* 
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(DATABASE_SOURCE))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    sentiments = JsonConvert.DeserializeObject<List<SentimentModel>>(apiResponse);
                }
            }
            */

            //Dummy sentiment data
            sentiments.Add(new SentimentModel { Id = 1, Timestamp = new DateTime(2023, 2, 21, 20, 28, 0), TextSearched = "example", SentimentResult = "postive", PercentageScore = 0.23});
            sentiments.Add(new SentimentModel { Id = 2, Timestamp = new DateTime(2023, 2, 21, 20, 29, 0), TextSearched = "test", SentimentResult = "negative", PercentageScore = 0.57});
            sentiments.Add(new SentimentModel { Id = 3, Timestamp = new DateTime(2023, 2, 21, 20, 30, 0), TextSearched = "another", SentimentResult = "neutral", PercentageScore = 0.98});

            return View(sentiments);
        }
    }
}