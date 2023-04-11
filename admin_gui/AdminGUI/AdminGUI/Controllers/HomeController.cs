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

        public IActionResult Analysis()
        {
            //Get all the sentiments
            List<SentimentModel>? sentiments = _dataSource.GetSentiments();

            //Calculate the analysis variables
            double avgPercentScore = 0;
            int numPos = 0;
            int numNeg = 0;
            int numNeu = 0;
            double percPos = 0;
            double percNeg = 0;
            double percNeu = 0;

            for(int i = 0; i < sentiments.Count; i++)
            {
                avgPercentScore += sentiments[i].PercentageScore;

                switch(sentiments[i].SentimentResult)
                {
                    case "positive":
                        numPos++;
                        break;
                    case "negative":
                        numNeg++;
                        break;
                    case "neutral":
                        numNeu++;
                        break;
                }
            }

            avgPercentScore /= sentiments.Count;
            percPos = (double)numPos / sentiments.Count;
            percNeg = (double)numNeg / sentiments.Count;
            percNeu = (double)numNeu / sentiments.Count;

            AnalysisModel analysis = new AnalysisModel
            {
                AvgSentimentScore = avgPercentScore,
                NumPositiveSentiments = numPos,
                NumNegativeSentiments = numNeg,
                NumNeutralSentiments = numNeu,
                PercentPositiveSentiments = percPos,
                PercentNegativeSentiments = percNeg,
                PercentNeutralSentiments = percNeu
            };

            return View("Analysis", analysis);
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