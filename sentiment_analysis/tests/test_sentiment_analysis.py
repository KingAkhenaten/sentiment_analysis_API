from sentiment_analysis.src.nltk_utils import do_analysis, format_analysis
from sentiment_analysis.server import app
import pytest

INPUT = "this is a sentence"

@pytest.fixture
def get_analysis():
    return do_analysis(INPUT)

def test_do_analysis_none():
    assert do_analysis(None) == TypeError


def test_do_analysis_norm(get_analysis):
    assert type(get_analysis) == dict


def test_format_analysis_none():
    assert format_analysis(None) == TypeError


def test_format_analyze(get_analysis):
    f_analysis = format_analysis(get_analysis)
    assert type(f_analysis['result']) == str


def test_post_analyze():
    form = {"sentence": INPUT}
    response = app.test_client().post('/analyze', json=form)
    assert response.status_code == 200
