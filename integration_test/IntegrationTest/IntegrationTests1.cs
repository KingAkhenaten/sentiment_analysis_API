using Newtonsoft.Json.Linq;
using Npgsql;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;



namespace IntegrationTest
{
    public class Tests
    {
        private const string PythonApiUrl = @"http://5400-project-sentiment_analysis-1:8000/analyze";
        //private const string PythonApiUrl = @"http://localhost:8000/analyze";
        private const string DbConnectionString = "Server=5400-project-db-1;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;";
        //private const string DbConnectionString = "Server=localhost:5432;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;";

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




        //[Test]
        //public async Task SendRequesToPerformAnalysis()
        //{

        //    // Send a request from the C# GUI to the Python API to perform sentiment analysis on a sample text
        //    var request = new HttpRequestMessage(HttpMethod.Post, PythonApiUrl);
        //    request.Content = new StringContent($"{{\"sentence\":\"{"I hate apples"}\"}}", Encoding.UTF8, "application/json");
        //    var response = await _httpClient.SendAsync(request);


        //    // Verify that the Python API returns the correct sentiment analysis results
        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        //    var responseContent = await response.Content.ReadAsStringAsync();
        //    Assert.That(responseContent, Is.EqualTo("{\"sentiment\":\"neg\"}"));


        //}


    }
}
