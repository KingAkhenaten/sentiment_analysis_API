///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  ClientGUI - Sentiment Analysis Project
//	File Name:         HomeController.cs
//	Description:       The main file of the client-side program - routes and sends requests between this service,
//                     the database, and the sentiment API.
//	Course:            CSCI-5400 - Software Production
//	Author:            Katie Wilson, wilsonkl4@etsu.edu, East Tennessee State University
//	Last Modified:     02/28/23
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

        //private string SENTIMENT_SOURCE = @"http://host.docker.internal:8000/analyze"; 
        //private string connString = "Server=host.docker.internal;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;";
        private string SENTIMENT_SOURCE = @"http://5400-project-sentiment_analysis-1:8000/analyze";
        private string connString = "Server=5400-project-db-1;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;";

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

            //If there are no sentiments, set the List to null (to allow for different view in Index)
            if (sentiments.Count == 0)
                sentiments = null;

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
                string json = "{\"sentence\": \"" + s.Sentence + "\"}";
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                //Send it to the API and ask to analyze
                using (var response = await httpClient.PostAsync(SENTIMENT_SOURCE, content))
                {
                    //Read the response from the API
                    System.Diagnostics.Debug.WriteLine($"Response: {response.ToString()}");

                    string res = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"{res}");

                    //Parse the response from the API
                    //{"result": "{amount * 100}% {polarity}"}
                    string val = res.Split("\"")[3];
                    string percentage = val.Split("%")[0].Trim();
                    double perc = (double.Parse(percentage) / 100.0);
                    string score = val.Split("%")[1].Trim();

                    System.Diagnostics.Debug.WriteLine($"{perc}");
                    System.Diagnostics.Debug.WriteLine($"{score}");

                    string scoreFull = "";
                    switch(score)
                    {
                        case "pos":
                            scoreFull = "positive";
                            break;
                        case "neg":
                            scoreFull = "negative";
                            break;
                        case "neu":
                            scoreFull = "neutral";
                            break;
                    }


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
                    cmd.Parameters.AddWithValue("@SentimentScore", scoreFull);
                    cmd.Parameters.AddWithValue("@SentimentPercentage", perc);

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

        public async Task<IActionResult> Delete(int id)
        {
            //Connect to the DB
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            conn.Open();

            //Ask the DB to delete the sentiment with the given Id
            string query = "DELETE FROM SentimentAnalysis WHERE ID = @Id;";

            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (NpgsqlException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to delete in DB sentiment with ID {id}: {e.Message}");
            }

            //Close the DB connection
            conn.Close();

            //Then, we want to go back to Index
            return RedirectToAction("Index");
        }
    }
}