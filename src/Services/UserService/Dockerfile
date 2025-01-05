FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./src/BuildingBlocks/CommonContracts/CommonContracts.csproj ./BuildingBlocks/CommonContracts/
COPY ./src/BuildingBlocks/EventBus/EventBus.csproj ./BuildingBlocks/EventBus/
COPY ./src/Services/UserService/UserService.Api/UserService.Api.csproj ./Services/UserService/UserService.Api/
COPY ./src/Services/UserService/UserService.Domain/UserService.Domain.csproj ./Services/UserService/UserService.Domain/
COPY ./src/Services/UserService/UserService.DAL/UserService.DAL.csproj ./Services/UserService/UserService.DAL/

RUN dotnet restore ./Services/UserService/UserService.Api/UserService.Api.csproj

COPY ./src .
RUN dotnet build ./Services/UserService/UserService.Api/UserService.Api.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "UserService.Api.dll"]
