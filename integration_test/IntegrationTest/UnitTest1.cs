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
        //private const string PythonApiUrl = @"http://5400-project-sentiment_analysis-1:8000/analyze";
        private const string PythonApiUrl = @"http://localhost:8000/analyze";
        private const string DbConnectionString = "Server=5400-project-db-1;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;";
        //private const string DbConnectionString = "Server=;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;";

        private HttpClient _httpClient = new HttpClient();
        private NpgsqlConnection conn = new NpgsqlConnection(DbConnectionString);



        [OneTimeSetUp]
        public void Initialize()
        {
           
            // Set up a connection to the PostgreSQL database
            
            this.conn.Open();
        }


        [OneTimeTearDown]
        public void Cleanup()
        {
            // Close the HTTP client and the database connection
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


            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM results WHERE inputText=@inputText", conn);
            command.Parameters.AddWithValue("inputText", inputText);
            NpgsqlDataReader reader = command.ExecuteReader();
            reader.Read();
            Assert.AreEqual(reader.GetString(1), "pos");
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