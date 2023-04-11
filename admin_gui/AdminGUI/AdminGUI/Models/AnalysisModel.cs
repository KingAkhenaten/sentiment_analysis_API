using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AdminGUI.Models
{
    public class AnalysisModel
    {
        [Display(Name = "Average Sentiment Score")]
        public double AvgSentimentScore { get; set; }

        [Display(Name = "Number of Positive Sentiments")]
        public int NumPositiveSentiments { get; set; }
        
        [Display(Name = "Percentage of Positive Sentiments")]
        public double PercentPositiveSentiments { get; set; }
        
        [Display(Name = "Number of Negative Sentiments")]
        public int NumNegativeSentiments { get; set; }
        
        [Display(Name = "Percentage of Negative Sentiments")]
        public double PercentNegativeSentiments { get; set; }
        
        [Display(Name = "Number of Neutral Sentiments")]
        public int NumNeutralSentiments { get; set; }
        
        [Display(Name = "Percentage of Neutral Sentiments")]
        public double PercentNeutralSentiments { get; set; }
    }
}
