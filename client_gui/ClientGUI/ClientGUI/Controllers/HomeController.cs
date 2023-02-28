using ClientGUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace ClientGUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private string SENTIMENT_SOURCE = @"http://host.docker.internal:8000/analyze";
        //private string SENTIMENT_SOURCE = @"https://localhost:8000/analyze";
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SentenceModel s)
        {
            System.Diagnostics.Debug.WriteLine($"Sentence: {s.Sentence}");

            using (var httpClient = new HttpClient())
            {
                //Package up the sentence to send
                //StringContent content = new StringContent(s.Sentence, Encoding.UTF8, "application/json");
                string json = "{\"sentence\": \"" + s.Sentence + "\"}";
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                //StringContent content = new StringContent(JsonConvert.SerializeObject(s), Encoding.UTF8, "application/json");

                //Send it to the API and ask to analyze
                using (var response = await httpClient.PostAsync(SENTIMENT_SOURCE, content))
                {
                    System.Diagnostics.Debug.WriteLine($"Response: {response.ToString()}");
                    string res = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"{res}");

                    /*
                    //Receive analysis back + package to send to DB
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    StringContent content2 = new StringContent(JsonConvert.SerializeObject(apiResponse), Encoding.UTF8, "application/json");

                    //Send the response to the DB
                    using (var response2 = await httpClient.PostAsync(DATABASE_SOURCE, content2))
                    {
                        string apiResponse2 = await response.Content.ReadAsStringAsync();
                    }
                    */
                }
            }

            //Then, we want to go back to Index
            return RedirectToAction("Index");
        }
    }
}