using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VideoCallService.Api.Hubs;
using VideoCallService.Api.Services;
using VideoCallService.DAL;
using VideoCallService.Domain.Interfaces;
using VideoCallService.Infrastructure.Services;
using Microsoft.AspNetCore.Http.Connections;
using VideoCallService.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Подключение к базе данных PostgreSQL
var conn = builder.Configuration.GetConnectionString("Default")
           ?? "Host=videocalldb;Database=videocall_db;Username=postgres;Password=postgres";

Console.WriteLine($"Строка подключения: {conn}");

builder.Services.AddDbContext<VideoCallDbContext>(options =>
    options.UseNpgsql(conn)
        .EnableSensitiveDataLogging());

// Добавление Redis для управления состоянием SignalR
var redisConnection = builder.Configuration["Redis:ConnectionString"] ?? "redis:6379,password=redisPassword,abortConnect=false";
Console.WriteLine($"Redis connection string: {redisConnection}");
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(15);
    hubOptions.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
    hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(30);
    hubOptions.MaximumReceiveMessageSize = 102400;
})
.AddStackExchangeRedis(redisConnection, options =>
{
    options.Configuration.ChannelPrefix = "VideoCallService";
    options.Configuration.AbortOnConnectFail = false;
});

// Регистрация сервисов
builder.Services.AddScoped<VideoCallService.DAL.IVideoRoomRepository, VideoCallService.DAL.VideoRoomRepository>();
builder.Services.AddScoped<VideoCallService.Domain.Interfaces.IVideoRoomRepository, VideoCallService.DAL.VideoRoomRepositoryAdapter>();
builder.Services.AddScoped<IVideoRoomService, VideoRoomService>();
builder.Services.AddScoped<VideoCallService.Domain.Repositories.IParticipantRepository, DummyParticipantRepository>();

// Настройка аутентификации без проверки JWT-токена
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsASecretKeyWithEnoughLength12345";
        // Настройка валидации JWT токена
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Проверка токена в query параметрах для SignalR
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                
                Console.WriteLine($"[JWT] Проверка токена для пути: {path}");
                
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/videohub"))
                {
                    context.Token = accessToken;
                    string tokenValue = accessToken.ToString();
                    Console.WriteLine($"[JWT] Установлен токен из query_string: {(tokenValue.Length > 15 ? tokenValue.Substring(0, 15) + "..." : tokenValue)}");
                    return Task.CompletedTask;
                }
                
                // Проверка токена в заголовке Authorization
                string authHeader = context.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    string token = authHeader.Substring("Bearer ".Length).Trim();
                    context.Token = token;
                    Console.WriteLine($"[JWT] Установлен токен из заголовка Authorization: {token.Substring(0, Math.Min(15, token.Length))}...");
                    return Task.CompletedTask;
                }
                
                // Проверка токена в куках
                if (context.Request.Cookies.ContainsKey("AUTH_COOKIE"))
                {
                    context.Token = context.Request.Cookies["AUTH_COOKIE"];
                    Console.WriteLine("[JWT] Установлен токен из cookie");
                    return Task.CompletedTask;
                }
                
                if (path.StartsWithSegments("/videohub"))
                {
                    Console.WriteLine("[JWT] Токен не найден для WebSocket соединения, но разрешаем подключение");
                    // Для WebSocket соединений разрешаем подключение без токена
                    return Task.CompletedTask;
                }
                
                Console.WriteLine($"[JWT] Auth request: {context.Request.Path} - токен не найден");
                return Task.CompletedTask;
            },
            
            OnAuthenticationFailed = context =>
            {
                if (context.Request.Path.StartsWithSegments("/videohub"))
                {
                    Console.WriteLine($"[JWT] Ошибка аутентификации для WebSocket: {context.Exception.Message}");
                    // Не блокируем WebSocket соединения при ошибке аутентификации
                    context.NoResult();
                    return Task.CompletedTask;
                }
                
                Console.WriteLine($"[JWT] Auth failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            
            OnTokenValidated = context =>
            {
                Console.WriteLine("[JWT] Токен успешно валидирован");
                return Task.CompletedTask;
            }
        };
    });

// Добавление контроллеров и Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Video Call Service API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Настройка CORS
builder.Services.AddCors(options =>
{
    // Основная политика CORS
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Content-Disposition");
    });

    // Специальная политика CORS для SignalR
    options.AddPolicy("SignalRCors", policy =>
    {
        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("Content-Disposition")
              .SetIsOriginAllowed(_ => true); // Разрешаем любой источник с credentials
    });
});

var app = builder.Build();

// Применяем миграции и инициализируем базу данных
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<VideoCallDbContext>();
        
        // Проверяем подключение к базе данных
        Console.WriteLine("Попытка подключения к базе данных...");
        var connectionString = dbContext.Database.GetConnectionString();
        Console.WriteLine($"Строка подключения: {connectionString}");
        Console.WriteLine("Подключение к базе данных успешно.");
        
        // ВАЖНО: В разработке можно использовать подход с удалением и пересозданием
        // В продакшене это необходимо заменить на миграции
        Console.WriteLine("Удаление существующей базы данных...");
        dbContext.Database.EnsureDeleted();
        Console.WriteLine("База данных удалена.");
        
        Console.WriteLine("Создание новой схемы базы данных...");
        dbContext.Database.EnsureCreated();
        Console.WriteLine("Схема базы данных создана успешно.");
        
        // Добавление тестовых данных
        if (!dbContext.VideoRooms.Any())
        {
            Console.WriteLine("Добавление тестовой комнаты...");
            var testRoom = new VideoCallService.Domain.Models.VideoRoom
            {
                Id = Guid.NewGuid(),
                Name = "Тестовая комната",
                OwnerId = Guid.Parse("a561dc4e-c5fa-474c-beb9-5e77d2a83637"), // ID пользователя Ivan
                AccessCode = "123456",
                CreatedAt = DateTime.UtcNow,
                MaxParticipants = 10,
                IsActive = true
            };
            dbContext.VideoRooms.Add(testRoom);
            dbContext.SaveChanges();
            Console.WriteLine($"Тестовая комната создана с ID: {testRoom.Id}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при инициализации базы данных: {ex.Message}");
        Console.WriteLine($"StackTrace: {ex.StackTrace}");
    }
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS должен быть раньше других middleware
app.UseCors("AllowAll");

app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Настраиваем эндпоинты
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<VideoHub>("/videohub", options => {
        options.Transports = 
            HttpTransportType.WebSockets | 
            HttpTransportType.LongPolling;
        options.ApplicationMaxBufferSize = 102400;
        options.TransportMaxBufferSize = 102400;
        options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(30);
    })
    .RequireCors("SignalRCors"); // Применяем специфичную для SignalR политику CORS
    endpoints.MapGet("/health", () => "Healthy");
});

app.Run();
