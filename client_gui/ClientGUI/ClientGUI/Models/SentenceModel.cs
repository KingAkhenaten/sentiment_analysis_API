///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  ClientGUI - Sentiment Analysis Project
//	File Name:         SentenceModel.cs
//	Description:       Defines the data model for a sentence, which is just the text of the sentence to send to the
//	                   API.
//	Course:            CSCI-5400 - Software Production
//	Author:            Katie Wilson, wilsonkl4@etsu.edu, East Tennessee State University
//	Last Modified:     02/28/23
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace ClientGUI.Models
{
    public class SentenceModel
    {
        public string? Sentence { get; set; }
    }
}
