///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  ClientGUI - Sentiment Analysis Project
//	File Name:         IntegrationTests.cs
//	Description:       Integration tests for the application.
//	Course:            CSCI-5400 - Software Production
//	Author:            Caleb Ishola
//	                   Katie Wilson, wilsonkl4@etsu.edu, East Tennessee State University
//	Last Modified:     03/28/23
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using ClientGUI.Connectors;
using ClientGUI.Controllers;
using ClientGUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Npgsql;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;



namespace IntegrationTest
{
    public class IntegrationTests
    {
        //private const string PythonApiUrl = @"http://5400-project-sentiment_analysis-1:8000/analyze";
        private const string PythonApiUrl = @"http://localhost:8000/analyze";
        //private const string DbConnectionString = "Server=5400-project-db-1;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;";
        private const string DbConnectionString = "Server=localhost:5432;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;";

        private HttpClient _httpClient;
        private NpgsqlConnection conn;


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this._httpClient =  new HttpClient();
            this.conn = new NpgsqlConnection(DbConnectionString);
            this.conn.Open();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this._httpClient.Dispose();
            this.conn.Dispose();

        }



        [Test]
        public async Task SentimentShouldBePositiveSentiment()
        {
            // Arrange
            var inputText = "I love this product!";
            var requestBody = new StringContent($"{{\"sentence\":\"{inputText}\"}}", Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync(PythonApiUrl, requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.IsNotNull(responseContent);


            // Additional assertions
            Assert.IsTrue(responseContent.Contains("pos"));
            // TODO: Implement additional assertions based on the expected response format
        }

        [Test]
        public async Task SentimentShouldBeNegetive()
        {
            // Arrange
            var inputText = "I hate this product!";
            var requestBody = new StringContent($"{{\"sentence\":\"{inputText}\"}}", Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync(PythonApiUrl, requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.IsNotNull(responseContent);


            // Additional assertions
            Assert.IsTrue(responseContent.Contains("neg"));
            // TODO: Implement additional assertions based on the expected response format
        }


        [Test]
        public async Task SentimentShouldBeNeuteral()
        {
            // Arrange
            var inputText = "This product!";
            var requestBody = new StringContent($"{{\"sentence\":\"{inputText}\"}}", Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync(PythonApiUrl, requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.IsNotNull(responseContent);


            // Additional assertions
            Assert.IsTrue(responseContent.Contains("neu"));
            // TODO: Implement additional assertions based on the expected response format
        }


        [Test]
        public async Task ShouldAddPositiveSentimentToDataBase()
        {

            // Arrange
            var inputText = "I love this product!";
            var requestBody = new StringContent($"{{\"sentence\":\"{inputText}\"}}", Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync(PythonApiUrl, requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.IsNotNull(responseContent);


            // Additional assertions
            Assert.IsTrue(responseContent.Contains("pos"));

            // Insert sentiment into database
            var query = "INSERT INTO SentimentAnalysis (TimeStamp, Text, SentimentScore, SentimentPercentage) " +
                        "VALUES (@TimeStamp, @Text, @SentimentScore, @SentimentPercentage);";
            using var cmd = new NpgsqlCommand(query, conn);
            
            cmd.Parameters.AddWithValue("@TimeStamp", DateTime.Now);
            cmd.Parameters.AddWithValue("@Text", inputText);
            cmd.Parameters.AddWithValue("@SentimentScore", "pos");
            cmd.Parameters.AddWithValue("@SentimentPercentage", 80.0);
            await cmd.ExecuteNonQueryAsync();

            // Assert that sentiment was inserted
            var selectQuery = "SELECT Text FROM SentimentAnalysis WHERE Text = @Text;";
            using var selectCmd = new NpgsqlCommand(selectQuery, conn);
            selectCmd.Parameters.AddWithValue("@Text", inputText);
            using var reader = await selectCmd.ExecuteReaderAsync();
            Assert.That(reader.HasRows, Is.True);
            reader.Read();
            Assert.That(reader.GetString(0), Is.EqualTo("I love this product!"));


           

        }

        [Test]
        public async Task ShouldAddNegativeSentimentToDataBase()
        {

            // Arrange
            var inputText = "I hate this product!";
            var requestBody = new StringContent($"{{\"sentence\":\"{inputText}\"}}", Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync(PythonApiUrl, requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.IsNotNull(responseContent);


            // Additional assertions
            Assert.IsTrue(responseContent.Contains("neg"));

            // Insert sentiment into database
            var query = "INSERT INTO SentimentAnalysis (TimeStamp, Text, SentimentScore, SentimentPercentage) " +
                        "VALUES (@TimeStamp, @Text, @SentimentScore, @SentimentPercentage);";
            using var cmd = new NpgsqlCommand(query, conn);
            
            cmd.Parameters.AddWithValue("@TimeStamp", DateTime.Now);
            cmd.Parameters.AddWithValue("@Text", inputText);
            cmd.Parameters.AddWithValue("@SentimentScore", "neg");
            cmd.Parameters.AddWithValue("@SentimentPercentage", 80.0);
            await cmd.ExecuteNonQueryAsync();

            // Assert that sentiment was inserted
            var selectQuery = "SELECT Text FROM SentimentAnalysis WHERE Text = @Text;";
            using var selectCmd = new NpgsqlCommand(selectQuery, conn);
            selectCmd.Parameters.AddWithValue("@Text", inputText);
            using var reader = await selectCmd.ExecuteReaderAsync();
            Assert.That(reader.HasRows, Is.True);
            reader.Read();
            Assert.That(reader.GetString(0), Is.EqualTo("I hate this product!"));
           

        }

        [Test]
        public async Task ShouldAddNeutralSentimentToDataBase()
        {

            // Arrange
            var inputText = "This product!";
            var requestBody = new StringContent($"{{\"sentence\":\"{inputText}\"}}", Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync(PythonApiUrl, requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.IsNotNull(responseContent);


            // Additional assertions
            Assert.IsTrue(responseContent.Contains("neu"));

            // Insert sentiment into database
            var query = "INSERT INTO SentimentAnalysis (TimeStamp, Text, SentimentScore, SentimentPercentage) " +
                        "VALUES (@TimeStamp, @Text, @SentimentScore, @SentimentPercentage);";
            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@TimeStamp", DateTime.Now);
            cmd.Parameters.AddWithValue("@Text", inputText);
            cmd.Parameters.AddWithValue("@SentimentScore", "neu");
            cmd.Parameters.AddWithValue("@SentimentPercentage", 80.0);
            await cmd.ExecuteNonQueryAsync();

            // Assert that sentiment was inserted
            var selectQuery = "SELECT Text FROM SentimentAnalysis WHERE Text = @Text;";
            using var selectCmd = new NpgsqlCommand(selectQuery, conn);
            selectCmd.Parameters.AddWithValue("@Text", inputText);
            using var reader = await selectCmd.ExecuteReaderAsync();
            Assert.That(reader.HasRows, Is.True);
            reader.Read();
            Assert.That(reader.GetString(0), Is.EqualTo("This product!"));


        }



        [Test]
        public async Task ShouldDeletePositiveSentimentFromDataBase()
        {

            // Arrange
            var inputText = "I love this product!";
            var requestBody = new StringContent($"{{\"sentence\":\"{inputText}\"}}", Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync(PythonApiUrl, requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.IsNotNull(responseContent);


            // Additional assertions
            Assert.IsTrue(responseContent.Contains("pos"));

            // Insert sentiment into database
            var query = "INSERT INTO SentimentAnalysis (TimeStamp, Text, SentimentScore, SentimentPercentage) " +
                        "VALUES (@TimeStamp, @Text, @SentimentScore, @SentimentPercentage);";
            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@TimeStamp", DateTime.Now);
            cmd.Parameters.AddWithValue("@Text", inputText);
            cmd.Parameters.AddWithValue("@SentimentScore", "pos");
            cmd.Parameters.AddWithValue("@SentimentPercentage", 80.0);
            await cmd.ExecuteNonQueryAsync();

            // Assert that sentiment was inserted
            var selectQuery = "SELECT Text FROM SentimentAnalysis WHERE Text = @Text;";
            using var selectCmd = new NpgsqlCommand(selectQuery, conn);
            selectCmd.Parameters.AddWithValue("@Text", inputText);
            using var reader = await selectCmd.ExecuteReaderAsync();
            Assert.That(reader.HasRows, Is.True);
            reader.Read();
            Assert.That(reader.GetString(0), Is.EqualTo("I love this product!"));
            conn.Close();


            conn.Open();
            // Delete inserted sentiment from database
            var deleteQuery = "DELETE FROM SentimentAnalysis WHERE Text = @Text;";
            using var deleteCmd = new NpgsqlCommand(deleteQuery, conn);
            deleteCmd.Parameters.AddWithValue("@Text", inputText);
            await deleteCmd.ExecuteNonQueryAsync();

            // Assert that sentiment was deleted
            using var selectCmd2 = new NpgsqlCommand(selectQuery, conn);
            selectCmd2.Parameters.AddWithValue("@Text", inputText);
            using var reader2 = await selectCmd2.ExecuteReaderAsync();
            Assert.That(reader2.HasRows, Is.False);


        }

        [Test]
        public async Task ShouldDeleteNegativeSentimentFromDataBase()
        {

            // Arrange
            var inputText = "I hate this product!";
            var requestBody = new StringContent($"{{\"sentence\":\"{inputText}\"}}", Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync(PythonApiUrl, requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.IsNotNull(responseContent);


            // Additional assertions
            Assert.IsTrue(responseContent.Contains("neg"));

            // Insert sentiment into database
            var query = "INSERT INTO SentimentAnalysis (TimeStamp, Text, SentimentScore, SentimentPercentage) " +
                        "VALUES (@TimeStamp, @Text, @SentimentScore, @SentimentPercentage);";
            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@TimeStamp", DateTime.Now);
            cmd.Parameters.AddWithValue("@Text", inputText);
            cmd.Parameters.AddWithValue("@SentimentScore", "neg");
            cmd.Parameters.AddWithValue("@SentimentPercentage", 80.0);
            await cmd.ExecuteNonQueryAsync();

            // Assert that sentiment was inserted
            var selectQuery = "SELECT Text FROM SentimentAnalysis WHERE Text = @Text;";
            using var selectCmd = new NpgsqlCommand(selectQuery, conn);
            selectCmd.Parameters.AddWithValue("@Text", inputText);
            using var reader = await selectCmd.ExecuteReaderAsync();
            Assert.That(reader.HasRows, Is.True);
            reader.Read();
            Assert.That(reader.GetString(0), Is.EqualTo("I hate this product!"));
            conn.Close();


            conn.Open();
            // Delete inserted sentiment from database
            var deleteQuery = "DELETE FROM SentimentAnalysis WHERE Text = @Text;";
            using var deleteCmd = new NpgsqlCommand(deleteQuery, conn);
            deleteCmd.Parameters.AddWithValue("@Text", inputText);
            await deleteCmd.ExecuteNonQueryAsync();

            // Assert that sentiment was deleted
            using var selectCmd2 = new NpgsqlCommand(selectQuery, conn);
            selectCmd2.Parameters.AddWithValue("@Text", inputText);
            using var reader2 = await selectCmd2.ExecuteReaderAsync();
            Assert.That(reader2.HasRows, Is.False);


        }

        [Test]
        public async Task ShouldDeleteNeutralSentimentFromDataBase()
        {

            // Arrange
            var inputText = "This product!";
            var requestBody = new StringContent($"{{\"sentence\":\"{inputText}\"}}", Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync(PythonApiUrl, requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.IsNotNull(responseContent);


            // Additional assertions
            Assert.IsTrue(responseContent.Contains("neu"));

            // Insert sentiment into database
            var query = "INSERT INTO SentimentAnalysis (TimeStamp, Text, SentimentScore, SentimentPercentage) " +
                        "VALUES (@TimeStamp, @Text, @SentimentScore, @SentimentPercentage);";
            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@TimeStamp", DateTime.Now);
            cmd.Parameters.AddWithValue("@Text", inputText);
            cmd.Parameters.AddWithValue("@SentimentScore", "neu");
            cmd.Parameters.AddWithValue("@SentimentPercentage", 80.0);
            await cmd.ExecuteNonQueryAsync();

            // Assert that sentiment was inserted
            var selectQuery = "SELECT Text FROM SentimentAnalysis WHERE Text = @Text;";
            using var selectCmd = new NpgsqlCommand(selectQuery, conn);
            selectCmd.Parameters.AddWithValue("@Text", inputText);
            using var reader = await selectCmd.ExecuteReaderAsync();
            Assert.That(reader.HasRows, Is.True);
            reader.Read();
            Assert.That(reader.GetString(0), Is.EqualTo("This product!"));
            conn.Close();


            conn.Open();

            // Delete inserted sentiment from database
            var deleteQuery = "DELETE FROM SentimentAnalysis WHERE Text = @Text;";
            using var deleteCmd = new NpgsqlCommand(deleteQuery, conn);
            deleteCmd.Parameters.AddWithValue("@Text", inputText);
            await deleteCmd.ExecuteNonQueryAsync();

            // Assert that sentiment was deleted
            using var selectCmd2 = new NpgsqlCommand(selectQuery, conn);
            selectCmd2.Parameters.AddWithValue("@Text", inputText);
            using var reader2 = await selectCmd2.ExecuteReaderAsync();
            Assert.That(reader2.HasRows, Is.False);


        }


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

        //Commented out because delete functionality has been removed from client
        /*
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
        */




    }
}
