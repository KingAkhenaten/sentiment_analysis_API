from flask import Flask, request
from sentiment_analysis.src.nltk_utils import do_analysis, format_analysis

app = Flask(__name__)


@app.post('/analyze')
def analyze():
    data = request.json
    result = do_analysis(data['sentence'])

    return format_analysis(result)