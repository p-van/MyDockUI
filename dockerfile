FROM mcr.microsoft.com/dotnet/core/sdk:3.0 as builder

WORKDIR /app
COPY . /app

RUN dotnet restore
RUN dotnet publish -o ./allPubs

FROM mcr.microsoft.com/dotnet/core/aspnet:latest as run

WORKDIR /app
COPY --from=builder /app/allPubs ./
ENTRYPOINT ["dotnet", "MyDockUI.dll"]


# docker image build -t mydockui:latest .
# docker run -d -e InfoApiClient__Scheme='http' -e InfoApiClient__Host='localhost' -e InfoApiClient__Port='6000' -e ASPNETCORE_URLS='http://*:5000' -p 5000:5000 --name dockUI mydockui:latest