FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine

WORKDIR /Packages
COPY ./Packages/*.nupkg /Packages/

WORKDIR /app

#copy the code in
COPY ./MapApi /app

#build the site
RUN dotnet restore

#run the site
#RUN dotnet watch run --project ./app.csproj
ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:8004"]
