using ClientGUI.Models;

namespace ClientGUI.Connectors
{
    public interface ISentiment
    {
        string ConnectionString { get; set; }
        Task<string[]> CreateSentiment(SentenceModel s);
    }
}