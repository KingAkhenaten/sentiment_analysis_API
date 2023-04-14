///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  AdmintGUI - Sentiment Analysis Project
//	File Name:         AdminGUIUnitTests.cs
//	Description:       Unit tests for the AdminGUI application.
//	Course:            CSCI-5400 - Software Production
//	Author:            Brandon Rhyno, rhynob@etsu.edu, East Tennessee State University
//	Last Modified:     04/14/23
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using AdminGUI.Controllers;
using AdminGUI.Models;
using AdminGUI.Connectors;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace AdminGUIUnitTests
{
    public class AdminGUIUnitTests
    {
        [Test]
        public void MaintenacneShouldReturnListOfSentiments()
        {
            //Mock the database using Moq
            var mockDataSource = new Mock<IDataSource>();
            mockDataSource.Setup(x => x.GetSentiments()).Returns(GetTestListOfSentiments());

            // Create the system under test(sut) -the HomeController, using
            //the mocked services
            var sut = new HomeController(mockDataSource.Object);

            //Call the Maintenance method
            var actionResult = sut.Maintenance();

            //Ensure the view is returned
            var view = actionResult as ViewResult;
            Assert.NotNull(view);

            //Ensure that the view is for the Maintenance page
            Assert.That(view.ViewName, Is.EqualTo("Maintenance"));

            //Ensure the model is of the correct data type
            var model = view.Model as List<SentimentModel>;
            Assert.NotNull(model);

            //Assert that there are the correct amount of sentiments in the model
            Assert.That(model.Count, Is.EqualTo(3));

            //Assert that the model contains the correct list of sentiments
            List<SentimentModel> testSentiments = GetTestListOfSentiments();
            for (int i = 0; i < model.Count; i++)
            {
                Assert.That(model[i].Id, Is.EqualTo(testSentiments[i].Id));
                Assert.That(model[i].TextSearched, Is.EqualTo(testSentiments[i].TextSearched));
                Assert.That(model[i].SentimentResult, Is.EqualTo(testSentiments[i].SentimentResult));
                Assert.That(model[i].Timestamp, Is.EqualTo(testSentiments[i].Timestamp));
                Assert.That(model[i].PercentageScore, Is.EqualTo(testSentiments[i].PercentageScore));
            }
        }

        [Test]
        public void AnalysisShouldReturnanAlyticsOfSentiments()
        {
            //Mock the database using Moq
            var mockDataSource = new Mock<IDataSource>();
            mockDataSource.Setup(x => x.GetSentiments()).Returns(GetTestListOfSentiments());

            // Create the system under test(sut) -the HomeController, using
            //the mocked services
            var sut = new HomeController(mockDataSource.Object);

            //Call the Analysis method
            var actionResult = sut.Analysis();

            //Ensure the view is returned
            var view = actionResult as ViewResult;
            Assert.NotNull(view);

            //Ensure that the view is for the Analysis page
            Assert.That(view.ViewName, Is.EqualTo("Analysis"));

            //Ensure the model is of the correct data type
            var model = view.Model as AnalysisModel;
            Assert.NotNull(model);

            // Get the analysis results for the test sentiments
            List<SentimentModel> testSentiments = GetTestListOfSentiments();
            double avgScore = 0;
            double percentage = (double)1 / (double)3;
            foreach (var sentiment in testSentiments)
            {
                avgScore += sentiment.PercentageScore;
            }
            avgScore /= 3;
            
            //Assert that the analysis is correct
            Assert.That(model.AvgSentimentScore, Is.EqualTo(avgScore));
            Assert.That(model.NumPositiveSentiments, Is.EqualTo(1));
            Assert.That(model.PercentPositiveSentiments, Is.EqualTo(percentage));

            Assert.That(model.NumNegativeSentiments, Is.EqualTo(1));
            Assert.That(model.PercentNegativeSentiments, Is.EqualTo(percentage));
            
            Assert.That(model.NumNeutralSentiments, Is.EqualTo(1));
            Assert.That(model.PercentNeutralSentiments, Is.EqualTo(percentage));

        }

        [Test]
        public void EditShouldUpdateASentimentRecord()
        {
            //Mock the database using Moq
            var mockDataSource = new Mock<IDataSource>();
            mockDataSource.Setup(x => x.GetSentiment(0)).Returns(GetTestSentiment());
            mockDataSource.Setup(x => x.EditSentiment(0, "positive")).Returns(true);

            // Create the system under test(sut) -the HomeController, using
            //the mocked services
            var sut = new HomeController(mockDataSource.Object);

            // PART 1: Navigate to the Edit page
            //Call the Edit method with the first sentiment in the list
            var actionResult = sut.Edit(0);

            //Ensure the view is returned
            var view = actionResult as ViewResult;
            Assert.NotNull(view);

            //Ensure that the view is for the Edit page
            Assert.That(view.ViewName, Is.EqualTo("Edit"));

            //Ensure the model is of the correct data type
            var model = view.Model as SentimentModel;
            Assert.NotNull(model);

            // PART 2: Modify the model and update the database
            //Mock the user changing the sentiment result
            model.SentimentResult = "positive";

            //Save the new changes to the mock DB
            var actionResult2 = sut.Edit(model).Result;

            //Ensure that we redirect
            var redirect = actionResult2 as RedirectToActionResult;
            Assert.NotNull(redirect);

            //Ensure that the redirect is for the Maintenance page
            Assert.That(redirect.ActionName, Is.EqualTo("Maintenance"));

            // Get the original list before the model change
            List<SentimentModel> testSentiments = GetTestListOfSentiments();
            
            //Assert that the first sentiment has been updated
            Assert.That(model.Id, Is.EqualTo(testSentiments[0].Id));
            Assert.That(model.TextSearched, Is.EqualTo(testSentiments[0].TextSearched));
            Assert.That(model.SentimentResult, Is.Not.EqualTo(testSentiments[0].SentimentResult));  //This should be different
            Assert.That(model.Timestamp, Is.EqualTo(testSentiments[0].Timestamp));
            Assert.That(model.PercentageScore, Is.EqualTo(testSentiments[0].PercentageScore));
        }

        [Test]
        public void DeleteShouldRemoveASentimentRecord()
        {
            //Mock the database using Moq
            var mockDataSource = new Mock<IDataSource>();
            mockDataSource.Setup(x => x.RemoveSentiment(0)).Returns(true);
            mockDataSource.Setup(x => x.GetSentiments()).Returns(GetTestListOfSentiments());

            //Create the system under test (sut) - the HomeController, using
            //the mocked services
            var sut = new HomeController(mockDataSource.Object);

            //Call the Delete method
            var actionResult = sut.Delete(0).Result;

            //Ensure that we redirect
            var redirect = actionResult as RedirectToActionResult;
            Assert.NotNull(redirect);

            //Ensure that the redirect is for the Maintenance page
            Assert.That(redirect.ActionName, Is.EqualTo("Maintenance"));
        }

        private List<SentimentModel> GetTestListOfSentiments()
        {
            List<SentimentModel> sentiments = new List<SentimentModel>();

            sentiments.Add(
                new SentimentModel
                {
                    Id = 0,
                    Timestamp = new DateTime(2023, 03, 27),
                    TextSearched = "test sentence",
                    SentimentResult = "neutral",
                    PercentageScore = 0.5
                });
            sentiments.Add(
                new SentimentModel
                {
                    Id = 1,
                    Timestamp = new DateTime(2023, 03, 27),
                    TextSearched = "i like sentences",
                    SentimentResult = "positive",
                    PercentageScore = 0.7
                });
            sentiments.Add(
                new SentimentModel
                {
                    Id = 2,
                    Timestamp = new DateTime(2023, 03, 27),
                    TextSearched = "i hate sentences",
                    SentimentResult = "negative",
                    PercentageScore = 0.2
                });

            return sentiments;
        }

        private SentimentModel GetTestSentiment()
        {
            return new SentimentModel
            {
                Id = 0,
                Timestamp = new DateTime(2023, 03, 27),
                TextSearched = "test sentence",
                SentimentResult = "neutral",
                PercentageScore = 0.5
            };
        }
    }
}