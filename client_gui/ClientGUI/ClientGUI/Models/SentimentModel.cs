namespace ClientGUI.Models
{
    public class SentimentModel
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string? TextSearched { get; set; }

        public string? SentimentResult { get; set; }

        public double PercentageScore { get; set; }
    }
}
