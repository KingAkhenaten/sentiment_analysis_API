///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  AdminGUI - Sentiment Analysis Project
//	File Name:         IDataSource.cs
//	Description:       Interface to define needed methods for any data sources for this project.
//                     Interface was defined to allow for dependency injection in the controllers.
//	Course:            CSCI-5400 - Software Production
//	Author:            Katie Wilson, wilsonkl4@etsu.edu, East Tennessee State University
//	Last Modified:     03/28/23
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using AdminGUI.Models;

namespace AdminGUI.Connectors
{
    public interface IDataSource
    {
        string ConnectionString { get; set; }
        List<SentimentModel> GetSentiments();
        SentimentModel GetSentiment(int id);
        bool AddSentiment(string text, string sentimentScore, double percentage);
        bool RemoveSentiment(int id);
        bool EditSentiment(int id, string newSentimentScore);
    }
}