///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  ClientGUI - Sentiment Analysis Project
//	File Name:         ClientGUIUnitTests.cs
//	Description:       Unit tests for the ClientGUI application.
//	Course:            CSCI-5400 - Software Production
//	Author:            Katie Wilson, wilsonkl4@etsu.edu, East Tennessee State University
//	Last Modified:     03/28/23
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using ClientGUI.Controllers;
using ClientGUI.Models;
using ClientGUI.Connectors;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace ClientGUIUnitTesting
{
    public class ClientGUIUnitTests
    {
        [Test]
        public void IndexShouldReturnListOfSentiments()
        {
            //Mock the database and sentiment services using Moq
            var mockDataSource = new Mock<IDataSource>();
            mockDataSource.Setup(x => x.GetSentiments()).Returns(GetTestListOfSentiments());

            var mockSentimentAnalyzer = new Mock<ISentiment>();

            //Create the system under test (sut) - the HomeController, using
            //the mocked services
            var sut = new HomeController(mockDataSource.Object, mockSentimentAnalyzer.Object);

            //Call the Index method
            var actionResult = sut.Index().Result;

            //Ensure the view is returned
            var view = actionResult as ViewResult;
            Assert.NotNull(view);

            //Ensure that the view is for the Index page
            Assert.That(view.ViewName, Is.EqualTo("Index"));

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
        public void CreateNoArgsShouldReturnCreateView()
        {
            //Mock the database and sentiment services using Moq
            var mockDataSource = new Mock<IDataSource>();
            var mockSentimentAnalyzer = new Mock<ISentiment>();

            //Create the system under test (sut) - the HomeController, using
            //the mocked services
            var sut = new HomeController(mockDataSource.Object, mockSentimentAnalyzer.Object);

            //Call the create method
            var actionResult = sut.Create();

            //Ensure the view is returned
            var view = actionResult as ViewResult;
            Assert.NotNull(view);

            //Ensure that the view is for the Create page
            Assert.That(view.ViewName, Is.EqualTo("Create"));
        }

        [Test]
        public void CreateShouldCreateAndAddSentiment()
        {
            SentenceModel s = new SentenceModel { Sentence = "test sentence" };
            string[] resultsArr = { "test sentence", "neutral", "0.5" };
            Task<string[]> task = Task.FromResult(resultsArr);

            //Mock the database and sentiment services using Moq
            var mockDataSource = new Mock<IDataSource>();
            mockDataSource.Setup(x => x.AddSentiment("test sentence", "neutral", 0.5)).Returns(true);
            mockDataSource.Setup(x => x.GetSentiments()).Returns(GetTestListOfSentiments());

            var mockSentimentAnalyzer = new Mock<ISentiment>();
            mockSentimentAnalyzer.Setup(x => x.CreateSentiment(s)).Returns(task);

            //Create the system under test (sut) - the HomeController, using
            //the mocked services
            var sut = new HomeController(mockDataSource.Object, mockSentimentAnalyzer.Object);

            //Call the Create method
            var actionResult = sut.Create(s).Result;

            //Ensure that we redirect
            var redirect = actionResult as RedirectToActionResult;
            Assert.NotNull(redirect);

            //Ensure that the redirect is for the Index page
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        //Commented out since delete functionality has been removed from client
        /*
        [Test]
        public void DeleteShouldRemoveSentiment()
        {
            //Mock the database and sentiment services using Moq
            var mockDataSource = new Mock<IDataSource>();
            mockDataSource.Setup(x => x.RemoveSentiment(0)).Returns(true);
            mockDataSource.Setup(x => x.GetSentiments()).Returns(GetTestListOfSentiments());

            var mockSentimentAnalyzer = new Mock<ISentiment>();

            //Create the system under test (sut) - the HomeController, using
            //the mocked services
            var sut = new HomeController(mockDataSource.Object, mockSentimentAnalyzer.Object);

            //Call the Delete method
            var actionResult = sut.Delete(0).Result;

            //Ensure that we redirect
            var redirect = actionResult as RedirectToActionResult;
            Assert.NotNull(redirect);

            //Ensure that the redirect is for the Index page
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }
        */

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
    }
}