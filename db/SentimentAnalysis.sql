CREATE TABLE SentimentAnalysis (
    ID SERIAL PRIMARY KEY,
    TimeStamp timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Text varchar(1000),
    SentimentScore varchar(255),
    SentimentPercentage int
);