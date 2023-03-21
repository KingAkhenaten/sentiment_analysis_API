FROM ubuntu:latest

EXPOSE 8000

RUN apt update -y && \
    apt install -y python3 python3-pip

RUN pip install Flask nltk pytest
RUN python3 -m nltk.downloader vader_lexicon

COPY . /sentiment_analysis/

CMD flask --app sentiment_analysis/server run --debug --host 0.0.0.0 --port 8000

#docker build -t sentiment_analysis .
#docker run -p 8000:8000 sentiment_analysis