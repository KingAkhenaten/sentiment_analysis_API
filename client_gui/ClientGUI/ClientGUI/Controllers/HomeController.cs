using ClientGUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace ClientGUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private string SENTIMENT_SOURCE = @"http://host.docker.internal:8000/analyze"; 
        private string connString = "Server=host.docker.internal;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //Maintain a list of all the sentiments stored in the DB
            List<SentimentModel> sentiments = new List<SentimentModel>();

            //Connect to the DB
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            conn.Open();

            //Query the DB for all sentiment objects
            string query = "SELECT * FROM SentimentAnalysis";
            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    //Read each object from DB, form the SentimentModel object, add it to the list
                    object[] vals = new object[rdr.FieldCount];
                    rdr.GetValues(vals);
                    sentiments.Add(
                        new SentimentModel
                        {
                            Id = (int)vals[0],
                            Timestamp = (DateTime)vals[1],
                            TextSearched = (string)vals[2],
                            SentimentResult = (string)vals[3],
                            PercentageScore = (float)vals[4]
                        });
                }
            }

            //Close the DB connection
            conn.Close();

            //Dummy sentiment data
            //sentiments.Add(new SentimentModel { Id = 1, Timestamp = new DateTime(2023, 2, 21, 20, 28, 0), TextSearched = "example", SentimentResult = "postive", PercentageScore = 0.23});
            //sentiments.Add(new SentimentModel { Id = 2, Timestamp = new DateTime(2023, 2, 21, 20, 29, 0), TextSearched = "test", SentimentResult = "negative", PercentageScore = 0.57});
            //sentiments.Add(new SentimentModel { Id = 3, Timestamp = new DateTime(2023, 2, 21, 20, 30, 0), TextSearched = "another", SentimentResult = "neutral", PercentageScore = 0.98});

            //Return the index view, showing the queried sentiments in the list view
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
                //StringContent content = new StringContent(JsonConvert.SerializeObject(s), Encoding.UTF8, "application/json");
                string json = "{\"sentence\": \"" + s.Sentence + "\"}";
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                //Send it to the API and ask to analyze
                using (var response = await httpClient.PostAsync(SENTIMENT_SOURCE, content))
                {
                    //Read the response from the API
                    System.Diagnostics.Debug.WriteLine($"Response: {response.ToString()}");

                    string res = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"{res}");

                    //{'result': f'{amount * 100}% {polarity}'}
                    string val = res.Split("'")[3];
                    string percentage = val.Split("%")[0].Trim();
                    string score = val.Split("%")[1].Trim();

                    //Connect to the DB
                    NpgsqlConnection conn = new NpgsqlConnection(connString);
                    conn.Open();

                    //Ask the DB to add the sentiment result as a new row in the table
                    string query = "INSERT INTO SentimentAnalysis " +
                           "(TimeStamp, Text, SentimentScore, SentimentPercentage) " +
                           "VALUES (@TimeStamp, @Text, @SentimentScore, @SentimentPercentage);";

                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TimeStamp", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Text", s.Sentence);
                    cmd.Parameters.AddWithValue("@SentimentScore", score);
                    cmd.Parameters.AddWithValue("@SentimentPercentage", percentage);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (NpgsqlException e)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to create in DB: {e.Message}");
                    }

                    //Close the DB connection
                    conn.Close();
                }
            }

            //Then, we want to go back to Index
            return RedirectToAction("Index");
        }
    }
}