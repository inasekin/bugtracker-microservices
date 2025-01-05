FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./src/BuildingBlocks/CommonContracts/CommonContracts.csproj ./BuildingBlocks/CommonContracts/
COPY ./src/BuildingBlocks/EventBus/EventBus.csproj ./BuildingBlocks/EventBus/
COPY ./src/Services/ProjectService/Core/Core.csproj ./Services/ProjectService/Core/
COPY ./src/Services/ProjectService/DataAccess/DataAccess.csproj ./Services/ProjectService/DataAccess/
COPY ./src/Services/ProjectService/WebHost/WebHost.csproj ./Services/ProjectService/WebHost/

RUN dotnet restore ./Services/ProjectService/WebHost/WebHost.csproj

COPY ./src .
RUN dotnet build ./Services/ProjectService/WebHost/WebHost.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "WebHost.dll"]
