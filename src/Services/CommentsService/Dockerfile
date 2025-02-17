FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./src/BuildingBlocks/CommonContracts/CommonContracts.csproj ./BuildingBlocks/CommonContracts/
COPY ./src/BuildingBlocks/EventBus/EventBus.csproj ./BuildingBlocks/EventBus/
COPY ./src/Services/CommentsService/CommentsService.Api/CommentsService.Api.csproj ./Services/CommentsService/CommentsService.Api/
COPY ./src/Services/CommentsService/CommentsService.Domain/CommentsService.Domain.csproj ./Services/CommentsService/CommentsService.Domain/
COPY ./src/Services/CommentsService/CommentsService.DAL/CommentsService.DAL.csproj ./Services/CommentsService/CommentsService.DAL/

RUN dotnet restore ./Services/CommentsService/CommentsService.Api/CommentsService.Api.csproj

COPY ./src .
RUN dotnet build ./Services/CommentsService/CommentsService.Api/CommentsService.Api.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "CommentsService.Api.dll"]
