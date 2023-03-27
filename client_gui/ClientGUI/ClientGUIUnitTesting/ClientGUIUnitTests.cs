using ClientGUI.Controllers;
using ClientGUI.Models;
using ClientGUI.Connectors;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace ClientGUIUnitTesting
{
    public class ClientGUIUnitTests
    {
        private HomeController sut; //sut = system under test

        //Setup method is called before each test is ran
        [SetUp]
        public void Setup()
        {
            //Mock the database and sentiment services using Moq
            var mockDataSource = new Mock<IDataSource>();
            mockDataSource.Setup(x => x.GetSentiments()).Returns(GetTestListOfSentiments());

            var mockSentimentAnalyzer = new Mock<ISentiment>();

            //Create the system under test (sut) - the HomeController, using
            //the mocked services
            var sut = new HomeController(mockDataSource.Object, mockSentimentAnalyzer.Object);
        }

        [Test]
        public void IndexShouldReturnListOfSentiments()
        {
            //Call the Index method
            var actionResult = sut.Index().Result;

            //Ensure the view is returned
            var view = actionResult as ViewResult;
            Assert.NotNull(view);

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