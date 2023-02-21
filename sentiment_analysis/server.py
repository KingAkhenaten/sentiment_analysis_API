# send sentence
# send back data

from flask import Flask, request
from analyze import do_analysis

app = Flask(__name__)

@app.route("/")
def hello_world():
    return "<p>Hello, World!</p>"

@app.post('/analyze')
def analyze():
    data = request.json
    result = do_analysis(data['sentence'])
    return result