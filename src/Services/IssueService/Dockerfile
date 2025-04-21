FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./src/BuildingBlocks/CommonContracts/CommonContracts.csproj ./BuildingBlocks/CommonContracts/
COPY ./src/BuildingBlocks/EventBus/EventBus.csproj ./BuildingBlocks/EventBus/

COPY ./src/Services/IssueService/IssueService.Api/IssueService.Api.csproj ./Services/IssueService/IssueService.Api/
COPY ./src/Services/IssueService/IssueService.Domain/IssueService.Domain.csproj ./Services/IssueService/IssueService.Domain/
COPY ./src/Services/IssueService/IssueService.DAL/IssueService.DAL.csproj ./Services/IssueService/IssueService.DAL/

RUN dotnet restore ./Services/IssueService/IssueService.Api/IssueService.Api.csproj

COPY ./src .
RUN dotnet build ./Services/IssueService/IssueService.Api/IssueService.Api.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "IssueService.Api.dll"]
