FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем проектные файлы (.csproj)
COPY ["src/Services/VideoCallService/VideoCallService.Domain/VideoCallService.Domain.csproj", "VideoCallService.Domain/"]
COPY ["src/Services/VideoCallService/VideoCallService.DAL/VideoCallService.DAL.csproj", "VideoCallService.DAL/"]
COPY ["src/Services/VideoCallService/VideoCallService.Api/VideoCallService.Api.csproj", "VideoCallService.Api/"]
COPY ["src/Services/VideoCallService/VideoCallService.Infrastructure/VideoCallService.Infrastructure.csproj", "VideoCallService.Infrastructure/"]

# Восстанавливаем зависимости
RUN dotnet restore "VideoCallService.Api/VideoCallService.Api.csproj"

# Копируем исходный код
COPY src/Services/VideoCallService/ .

# Собираем проект
WORKDIR "/src/VideoCallService.Api"
RUN dotnet build "VideoCallService.Api.csproj" -c Release -o /app/build
RUN dotnet publish "VideoCallService.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Копируем appsettings
COPY --from=build /src/VideoCallService.Api/appsettings*.json ./

# JWT ключ должен быть передан через переменные окружения при запуске контейнера
# Например: docker run -e Jwt__Key="your-secret-key" videocallservice
# ENV Jwt__Key="ThisIsASecretKeyWithEnoughLength12345"

# Копируем собранное приложение
COPY --from=build /app/publish .

# Устанавливаем переменные среды
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 80
ENTRYPOINT ["dotnet", "VideoCallService.Api.dll"]
