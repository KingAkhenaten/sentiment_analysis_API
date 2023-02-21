from nltk.sentiment.vader import SentimentIntensityAnalyzer


def do_analysis(sentence:str=None) -> str:
    if sentence is None:
        return Exception("sentence not specified")

    sid = SentimentIntensityAnalyzer()
    ss = sid.polarity_scores(sentence)
    return ss