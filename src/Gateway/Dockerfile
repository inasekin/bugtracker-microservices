FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Установка curl
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY GatewayService.Api/GatewayService.Api.csproj GatewayService.Api/
RUN dotnet restore GatewayService.Api/GatewayService.Api.csproj
COPY . .
RUN dotnet build GatewayService.Api/GatewayService.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish GatewayService.Api/GatewayService.Api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GatewayService.Api.dll"]
