///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  ClientGUI - Sentiment Analysis Project
//	File Name:         PostgresDataSource.cs
//	Description:       Implementation of IDataSource that connects to our Postgres Database.
//	Course:            CSCI-5400 - Software Production
//	Author:            Katie Wilson, wilsonkl4@etsu.edu, East Tennessee State University
//	Last Modified:     03/28/23
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using ClientGUI.Models;
using Npgsql;

namespace ClientGUI.Connectors
{
    public class PostgresDataSource : IDataSource
    {
        private static string DATABASE_SOURCE = "Server=5400-project-db-1;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;";
        public string ConnectionString { get; set; }

        public PostgresDataSource()
        {
            ConnectionString = DATABASE_SOURCE;
        }

        public PostgresDataSource(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public List<SentimentModel> GetSentiments()
        {
            //Maintain a list of all the sentiments stored in the DB
            List<SentimentModel> sentiments = new List<SentimentModel>();

            //Connect to the DB
            NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
            conn.Open();

            //Query the DB for all sentiment objects
            string query = "SELECT * FROM SentimentAnalysis";
            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    //Read each object from DB, form the SentimentModel object, add it to the list
                    object[] vals = new object[rdr.FieldCount];
                    rdr.GetValues(vals);
                    sentiments.Add(
                        new SentimentModel
                        {
                            Id = (int)vals[0],
                            Timestamp = (DateTime)vals[1],
                            TextSearched = (string)vals[2],
                            SentimentResult = (string)vals[3],
                            PercentageScore = (float)vals[4]
                        });
                }
            }

            //Close the DB connection
            conn.Close();

            return sentiments;
        }

        public bool AddSentiment(string text, string sentimentScore, double percentage)
        {
            bool success = false;

            //Connect to the DB
            NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
            conn.Open();

            //Ask the DB to add the sentiment result as a new row in the table
            string query = "INSERT INTO SentimentAnalysis " +
                   "(TimeStamp, Text, SentimentScore, SentimentPercentage) " +
                   "VALUES (@TimeStamp, @Text, @SentimentScore, @SentimentPercentage);";

            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@TimeStamp", DateTime.Now);
            cmd.Parameters.AddWithValue("@Text", text);
            cmd.Parameters.AddWithValue("@SentimentScore", sentimentScore);
            cmd.Parameters.AddWithValue("@SentimentPercentage", percentage);

            try
            {
                cmd.ExecuteNonQuery();
                success = true;
            }
            catch (NpgsqlException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to create in DB: {e.Message}");
            }

            //Close the DB connection
            conn.Close();

            return success;
        }

        public bool RemoveSentiment(int id)
        {
            bool success = false;

            //Connect to the DB
            NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
            conn.Open();

            //Ask the DB to delete the sentiment with the given Id
            string query = "DELETE FROM SentimentAnalysis WHERE ID = @Id;";

            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            try
            {
                cmd.ExecuteNonQuery();
                success = true;
            }
            catch (NpgsqlException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to delete in DB sentiment with ID {id}: {e.Message}");
            }

            //Close the DB connection
            conn.Close();

            return success;
        }
    }
}