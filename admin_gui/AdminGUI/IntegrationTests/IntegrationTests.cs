///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  AdmintGUI - Sentiment Analysis Project
//	File Name:         AdminGUIUnitTests.cs
//	Description:       Unit tests for the AdminGUI application.
//	Course:            CSCI-5400 - Software Production
//	Author:            Caleb Ishola, isholaa@etsu.edu, East Tennessee State University
//	Last Modified:     04/15/23
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using AdminGUI.Connectors;
using AdminGUI.Controllers;
using AdminGUI.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Npgsql;
using System.Text;

namespace IntegrationTest
{
    public class Tests
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
            this._httpClient = new HttpClient();
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
        public async Task ShouldAddPositiveSentimentToDataBase()
        {
            //Create the system under test (sut) - the HomeController, using the real services
            //var sut = new HomeController(new PostgresDataSource(), new SentimentAnalyzer());
            var sut = new HomeController(new PostgresDataSource("Server=localhost;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;"));
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
            var query = "INSERT INTO SentimentAnalysis (Id, TimeStamp, Text, SentimentScore, SentimentPercentage) " +
                        "VALUES (@Id, @TimeStamp, @Text, @SentimentScore, @SentimentPercentage);";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", 0);
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


            //Call the Delete method- we do nor need in the db any more 
            var actionResult = sut.Delete(0).Result;

        }



        [Test]
        public async Task ShouldAddNegativeSentimentToDataBase()
        { 
            //Create the system under test (sut) - the HomeController, using the real services
            //var sut = new HomeController(new PostgresDataSource(), new SentimentAnalyzer());
            var sut = new HomeController(new PostgresDataSource("Server=localhost;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;"));

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
            var query = "INSERT INTO SentimentAnalysis (Id, TimeStamp, Text, SentimentScore, SentimentPercentage) " +
                        "VALUES (@Id, @TimeStamp, @Text, @SentimentScore, @SentimentPercentage);";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", 11);
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

            //Call the Delete method- we do nor need in the db any more 
            var actionResult = sut.Delete(11).Result;
        }

        [Test]
        public async Task ShouldAddNeutralSentimentToDataBase()
        {
            //Create the system under test (sut) - the HomeController, using the real services
            //var sut = new HomeController(new PostgresDataSource(), new SentimentAnalyzer());
            var sut = new HomeController(new PostgresDataSource("Server=localhost;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;"));
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
            var query = "INSERT INTO SentimentAnalysis (Id, TimeStamp, Text, SentimentScore, SentimentPercentage) " +
                        "VALUES (@Id, @TimeStamp, @Text, @SentimentScore, @SentimentPercentage);";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", 2);
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


            //Call the Delete method- we do nor need in the db any more 
            var actionResult = sut.Delete(2).Result;

        }
        //Above test runs making a total three sentiments added 
        //We check if the three sentiments were added 
        
        [Test]
        public async Task MaintenanceShouldReturnListOfSentiments()
        {
            //Create the system under test (sut) - the HomeController, using the real services
            //var sut = new HomeController(new PostgresDataSource(), new SentimentAnalyzer());
            var sut = new HomeController(new PostgresDataSource("Server=localhost;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;"));
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
            var query = "INSERT INTO SentimentAnalysis (Id, TimeStamp, Text, SentimentScore, SentimentPercentage) " +
                        "VALUES (@Id, @TimeStamp, @Text, @SentimentScore, @SentimentPercentage);";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", 111);
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
            Assert.That(model.Count, Is.EqualTo(1));


            //Call the Delete method- we do nor need in the db any more 
            var actionResult3 = sut.Delete(111).Result;
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
        public async Task DeleteShouldRemoveASentimentRecord()
        {
            
            //Create the system under test (sut) - the HomeController, using the real services
            //var sut = new HomeController(new PostgresDataSource(), new SentimentAnalyzer());
            var sut = new HomeController(new PostgresDataSource("Server=localhost;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;"));

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
            var query = "INSERT INTO SentimentAnalysis (Id, TimeStamp, Text, SentimentScore, SentimentPercentage) " +
                        "VALUES (@Id, @TimeStamp, @Text, @SentimentScore, @SentimentPercentage);";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", 3);
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


            //Call the Delete method
            var actionResult = sut.Delete(3).Result;

            //Ensure that we redirect
            var redirect = actionResult as RedirectToActionResult;
            Assert.NotNull(redirect);

            //Ensure that the redirect is for the Maintenance page
            Assert.That(redirect.ActionName, Is.EqualTo("Maintenance"));
        }



        [Test]
        public async Task EditShouldUpdateASentimentRecord()
        {
            //Create the system under test (sut) - the HomeController, using the real services
            //var sut = new HomeController(new PostgresDataSource(), new SentimentAnalyzer());
            var sut = new HomeController(new PostgresDataSource("Server=localhost;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;"));
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
            var query = "INSERT INTO SentimentAnalysis (Id, TimeStamp, Text, SentimentScore, SentimentPercentage) " +
                        "VALUES (@Id, @TimeStamp, @Text, @SentimentScore, @SentimentPercentage);";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", 4);
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



            // PART 1: Navigate to the Edit page
            //Call the Edit method with the first sentiment in the list
            var actionResult = sut.Edit(4);

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
            model.SentimentResult = "pos";

            //Save the new changes to the mock DB
            var actionResult2 = sut.Edit(model).Result;

            //Ensure that we redirect
            var redirect = actionResult2 as RedirectToActionResult;
            Assert.NotNull(redirect);

            //Ensure that the redirect is for the Maintenance page
            Assert.That(redirect.ActionName, Is.EqualTo("Maintenance"));

            Assert.That(model.SentimentResult, Is.Not.EqualTo("neg"));  //

            //Call the Delete method- we do nor need in the db any more 
            var actionResult3 = sut.Delete(4).Result;

        }

    }
}