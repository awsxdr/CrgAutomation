FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

COPY ** /src/
WORKDIR /src/

RUN dotnet build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS run

COPY --from=build /src/bin/Debug/net8.0/* .

RUN apt-get -y update
RUN apt-get -y install gconf-service libasound2 libatk1.0-0 libc6 libcairo2 libcups2 libdbus-1-3 libexpat1 libfontconfig1 libgcc1 libgconf-2-4 libgdk-pixbuf2.0-0 libglib2.0-0 libgtk-3-0 libnspr4 libpango-1.0-0 libpangocairo-1.0-0 libstdc++6 libx11-6 libx11-xcb1 libxcb1 libxcomposite1 libxcursor1 libxdamage1 libxext6 libxfixes3 libxi6 libxrandr2 libxrender1 libxss1 libxtst6 ca-certificates fonts-liberation libappindicator1 libnss3 lsb-release xdg-utils wget
RUN wget https://static.wftda.com/stats/wftda-statsbook-full-A4.xlsx

CMD [ "./CrgAutomation" ]