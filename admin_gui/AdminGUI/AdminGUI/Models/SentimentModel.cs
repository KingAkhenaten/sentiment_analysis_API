///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  AdminGUI - Sentiment Analysis Project
//	File Name:         SentimentModel.cs
//	Description:       Defines the data model for a sentiment result, including an id, timestamp, text searched, 
//                     sentiment result (pos/neg/neu), and percentage score.
//	Course:            CSCI-5400 - Software Production
//	Author:            Katie Wilson, wilsonkl4@etsu.edu, East Tennessee State University
//	Last Modified:     02/28/23
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace AdminGUI.Models
{
    public class SentimentModel
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        [Display(Name = "Text Searched")]
        public string? TextSearched { get; set; }

        [Display(Name = "Sentiment Result")]
        public string? SentimentResult { get; set; }

        [Display(Name = "Percentage Score")]
        public double PercentageScore { get; set; }
    }
}
