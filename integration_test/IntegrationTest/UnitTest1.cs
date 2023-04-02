using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;



namespace IntegrationTest
{
    public class Tests
    {
        private const string PythonApiUrl = @"http://5400-project-sentiment_analysis-1:8000/analyze";
       // private const string DbConnectionString = "Server=5400-project-db-1;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;";



        private  HttpClient? _httpClient;


        [Test]
        public async Task TestSentimentAnalysis()
        {
            // Send a request from the C# GUI to the Python API to perform sentiment analysis on a sample text
            var request = new HttpRequestMessage(HttpMethod.Post, PythonApiUrl);
            request.Content = new StringContent("{\"text\":\"I hate apples\"}", Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);


            // Verify that the Python API returns the correct sentiment analysis results
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.That(responseContent, Is.EqualTo("{\"sentiment\":\"n\"}"));


           
        }
    }
}