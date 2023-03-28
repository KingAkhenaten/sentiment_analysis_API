using ClientGUI.Models;
using System.Text;

namespace ClientGUI.Connectors
{
    public class SentimentAnalyzer : ISentiment
    {
        public string ConnectionString { get; set; }

        public SentimentAnalyzer(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public async Task<string[]> CreateSentiment(SentenceModel s)
        {
            string?[] results = new string[3];

            System.Diagnostics.Debug.WriteLine($"Sentence: {s.Sentence}");

            using (var httpClient = new HttpClient())
            {
                //Package up the sentence to send
                string json = "{\"sentence\": \"" + s.Sentence + "\"}";
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                //Send it to the API and ask to analyze
                using (var response = await httpClient.PostAsync(ConnectionString, content))
                {
                    //Read the response from the API
                    System.Diagnostics.Debug.WriteLine($"Response: {response.ToString()}");

                    string res = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"{res}");

                    //Parse the response from the API
                    //{"result": "{amount * 100}% {polarity}"}
                    string val = res.Split("\"")[3];
                    string percentage = val.Split("%")[0].Trim();
                    double perc = (double.Parse(percentage) / 100.0);
                    string score = val.Split("%")[1].Trim();

                    System.Diagnostics.Debug.WriteLine($"{perc}");
                    System.Diagnostics.Debug.WriteLine($"{score}");

                    string scoreFull = "";
                    switch (score)
                    {
                        case "pos":
                            scoreFull = "positive";
                            break;
                        case "neg":
                            scoreFull = "negative";
                            break;
                        case "neu":
                            scoreFull = "neutral";
                            break;
                    }

                    results[0] = s.Sentence;
                    results[1] = scoreFull;
                    results[2] = perc.ToString();
                }
            }

            return results;
        }
    }
}