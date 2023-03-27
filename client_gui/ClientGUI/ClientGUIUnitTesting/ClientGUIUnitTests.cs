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

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void IndexShouldReturnListOfSentiments()
        {
            var mockDataSource = new Mock<IDataSource>();
            mockDataSource.Setup(x => x.GetSentiments()).Returns(GetTestListOfSentiments());

            var mockSentimentAnalyzer = new Mock<ISentiment>();

            var sut = new HomeController(mockDataSource.Object, mockSentimentAnalyzer.Object);

            Task<IActionResult> result = sut.Index();
            IActionResult actionResult = result.Result;

            //Assert that the page loaded correctly
            Assert.IsInstanceOf(typeof(OkResult), actionResult);
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