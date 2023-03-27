using ClientGUI.Models;

namespace ClientGUI.Connectors
{
    public interface IDataSource
    {
        string ConnectionString { get; set; }
        List<SentimentModel> GetSentiments();
        bool AddSentiment(string text, string sentimentScore, double percentage);
        bool RemoveSentiment(int id);
    }
}