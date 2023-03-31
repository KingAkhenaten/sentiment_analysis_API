using ClientGUI.Controllers;
using ClientGUI.Models;
using ClientGUI.Connectors;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationTesting
{
    public class IntegrationTests
    {
        [TearDown]
        public void RemoveAllItems()
        {
            //Create the system under test (sut) - the HomeController, using the real services
            //var sut = new HomeController(new PostgresDataSource(), new SentimentAnalyzer());
            var sut = new HomeController(new PostgresDataSource("Server=localhost;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;"), new SentimentAnalyzer(@"http://localhost:8000/analyze"));

            //Call the Index method to get all the sentiments
            var actionResult = sut.Index().Result;
            var view = actionResult as ViewResult;
            var model = view.Model as List<SentimentModel>;

            //Delete them all
            if (model == null)
                return;

            for (int i = 0; i < model.Count; i++)
            {
                var r = sut.Delete(model[i].Id).Result;
            }
        }

        [Test]
        public void IndexShouldReturnEmptyListOfSentimentsWhenNew()
        {
            //Create the system under test (sut) - the HomeController, using the real services
            //var sut = new HomeController(new PostgresDataSource(), new SentimentAnalyzer());
            var sut = new HomeController(new PostgresDataSource("Server=localhost;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;"), new SentimentAnalyzer(@"http://localhost:8000/analyze"));

            //Call the Index method
            var actionResult = sut.Index().Result;

            //Ensure the view is returned
            var view = actionResult as ViewResult;
            Assert.NotNull(view);

            //Ensure that the view is for the Index page
            Assert.That(view.ViewName, Is.EqualTo("Index"));

            //Since there are no sentiments, it should be null
            var model = view.Model as List<SentimentModel>;
            Assert.Null(model);
        }

        [Test]
        public void IndexShouldReturnListOfSentimentsWhenSomeAdded()
        {
            //Create the system under test (sut) - the HomeController, using the real services
            //var sut = new HomeController(new PostgresDataSource(), new SentimentAnalyzer());
            var sut = new HomeController(new PostgresDataSource("Server=localhost;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;"), new SentimentAnalyzer(@"http://localhost:8000/analyze"));


            //Create and add 2 sentiments
            List<SentenceModel> sentences = new List<SentenceModel>();
            sentences.Add(new SentenceModel { Sentence = "i love sentences" });
            sentences.Add(new SentenceModel { Sentence = "i hate sentences" });
            var r1 = sut.Create(sentences[0]).Result;
            var r2 = sut.Create(sentences[1]).Result;

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

            //Assert that there are the correct amount of sentiments in the model - should be 2 since we added 2
            Assert.That(model.Count, Is.EqualTo(2));

            //Assert that the model contains the correct list of sentiments
            for (int i = 0; i < model.Count; i++)
            {
                Assert.That(model[i].TextSearched, Is.EqualTo(sentences[i].Sentence));
            }
        }

        [Test]
        public void CreateNoArgsShouldReturnCreateView()
        {
            //Create the system under test (sut) - the HomeController, using the real services
            //var sut = new HomeController(new PostgresDataSource(), new SentimentAnalyzer());
            var sut = new HomeController(new PostgresDataSource("Server=localhost;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;"), new SentimentAnalyzer(@"http://localhost:8000/analyze"));

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

            //Create the system under test (sut) - the HomeController, using the real services
            //var sut = new HomeController(new PostgresDataSource(), new SentimentAnalyzer());
            var sut = new HomeController(new PostgresDataSource("Server=localhost;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;"), new SentimentAnalyzer(@"http://localhost:8000/analyze"));

            //Call the Create method
            var actionResult = sut.Create(s).Result;

            //Ensure that we redirect
            var redirect = actionResult as RedirectToActionResult;
            Assert.NotNull(redirect);

            //Ensure that the redirect is for the Index page
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));

            //Call the Index method
            actionResult = sut.Index().Result;

            //Ensure the view is returned
            var view = actionResult as ViewResult;
            Assert.NotNull(view);

            //Ensure that the view is for the Index page
            Assert.That(view.ViewName, Is.EqualTo("Index"));

            //Ensure the model is of the correct data type
            var model = view.Model as List<SentimentModel>;
            Assert.NotNull(model);

            //Assert that there are the correct amount of sentiments in the model - should be 1, the one we added with create
            Assert.That(model.Count, Is.EqualTo(1));

            //Assert that the model contains the correct sentiment object
            Assert.That(model[0].TextSearched, Is.EqualTo(s.Sentence));
        }

        [Test]
        public void DeleteShouldRemoveSentiment()
        {
            //Create the system under test (sut) - the HomeController, using the real services
            //var sut = new HomeController(new PostgresDataSource(), new SentimentAnalyzer());
            var sut = new HomeController(new PostgresDataSource("Server=localhost;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;"), new SentimentAnalyzer(@"http://localhost:8000/analyze"));

            //Create a sentence, which we can create a sentiment out of to be able to delete it
            var r1 = sut.Create(new SentenceModel { Sentence = "i love sentences" }).Result;

            //Get the sentiments to figure out its Id
            var result = sut.Index().Result;
            var v = result as ViewResult;
            var m = v.Model as List<SentimentModel>;

            //Call the Delete method
            var actionResult = sut.Delete(m[0].Id).Result;

            //Ensure that we redirect
            var redirect = actionResult as RedirectToActionResult;
            Assert.NotNull(redirect);

            //Ensure that the redirect is for the Index page
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));

            //Call the Index method
            actionResult = sut.Index().Result;

            //Ensure the view is returned
            var view = actionResult as ViewResult;
            Assert.NotNull(view);

            //Ensure that the view is for the Index page
            Assert.That(view.ViewName, Is.EqualTo("Index"));

            //Model should be null since there shouldn't be anything in there
            var model = view.Model as List<SentimentModel>;
            Assert.Null(model);
        }
    }
}