using ClientGUI.Models;

namespace ClientGUI.Connectors
{
    public interface ISentiment
    {
        string ConnectionString { get; set; }
        Task<string[]> CreateSentiment(SentenceModel s);

        //interfaces are made so I need to make implementation
        //of the interfaces with what is currently in my
        //home controller and then make the controller
        //use the interfaces instead so that I can mock them
        //for unit testing
    }
}