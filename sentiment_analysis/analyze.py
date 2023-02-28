from nltk.sentiment.vader import SentimentIntensityAnalyzer


def do_analysis(sentence:str=None) -> str:
    if sentence is None:
        return Exception("sentence not specified")

    sid = SentimentIntensityAnalyzer()
    ss = sid.polarity_scores(sentence)
    return ss


def format_analysis(ss: dict):
    polarity = max(ss, key=ss.get)
    amount = ss[max(ss, key=ss.get)]

    return {'result': f'{amount * 100}% {polarity}'}
