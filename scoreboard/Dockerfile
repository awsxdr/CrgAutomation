FROM ubuntu:latest

RUN apt-get -y update
RUN apt-get -y install curl wget unzip openjdk-8-jre

RUN mkdir /scoreboard/
WORKDIR /scoreboard/
RUN wget https://github.com/rollerderby/scoreboard/releases/download/v2023.7/crg-scoreboard_v2023.7.zip

COPY run-scoreboard.sh .
RUN chmod +x ./run-scoreboard.sh

CMD [ "./run-scoreboard.sh" ]