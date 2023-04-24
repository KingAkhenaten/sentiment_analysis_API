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
using ClientGUI.Connectors;

namespace ClientGUI.Controllers
{
    public class HomeController : Controller
    {
        //private string SENTIMENT_SOURCE = @"http://host.docker.internal:8000/analyze"; 
        //private string connString = "Server=host.docker.internal;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;";
        //private string SENTIMENT_SOURCE = @"http://5400-project-sentiment_analysis-1:8000/analyze";
        //private string DATABASE_SOURCE = "Server=5400-project-db-1;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;";

        private IDataSource _dataSource;
        private ISentiment _sentimentAnalyzer;

        public HomeController(IDataSource dataSource, ISentiment sentimentAnalyzer)
        {
            _dataSource = dataSource;
            _sentimentAnalyzer = sentimentAnalyzer;
        }
        

        public async Task<IActionResult> Index()
        {
            /*
            //Query the database for the sentiments
            List<SentimentModel>? sentiments = _dataSource.GetSentiments();

            //If there are no sentiments, set the List to null (to allow for different view in Index)
            if (sentiments.Count == 0)
                sentiments = null;

            //Return the index view, showing the queried sentiments in the list view
            return View("Index", sentiments);
            */
        }

        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public async Task<IActionResult> Create(SentenceModel s)
        {
            //Create sentiment results by calling sentiment service
            string[] task = await _sentimentAnalyzer.CreateSentiment(s);

            //Insert sentiment into database table
            bool success = _dataSource.AddSentiment(task[0], task[1], double.Parse(task[2]));
            System.Diagnostics.Debug.WriteLine($"Insert succeeded: {success}");

            //Then, we want to go back to Index
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            //Remove sentiment from database table
            bool success = _dataSource.RemoveSentiment(id);
            System.Diagnostics.Debug.WriteLine($"Insert succeeded: {success}");

            //Then, we want to go back to Index
            return RedirectToAction("Index");
        }
    }
}