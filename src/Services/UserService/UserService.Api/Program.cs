using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Serilog.Events;

using UserService.Api.Services;
using UserService.DAL;
using UserService.Domain.Models;

using System.Text;
using EventBus;

var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "UserService")
    .WriteTo.Console()
    .WriteTo.File("logs/userservice-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var environment = builder.Environment.EnvironmentName;
Log.Information($"Среда выполнения: {environment}");

// Подключение к базе данных
var conn = builder.Configuration.GetConnectionString("Default")
           ?? "Host=userdb;Database=user_db;Username=postgres;Password=postgres";
if (string.IsNullOrEmpty(conn))
{
    throw new InvalidOperationException("Строка подключения 'DefaultConnection' не найдена.");
}

// Регистрация DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(conn)
        .EnableSensitiveDataLogging()
        .LogTo(Log.Information));
Log.Information("Успешное подключение к базе данных.");

var redisConnection = builder.Configuration["Redis:ConnectionString"] ?? "redis:6379,password=redisPassword";
var redisInstanceName = builder.Configuration["Redis:InstanceName"] ?? "UserService_";

// Добавляем Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
  options.Configuration = redisConnection;
  options.InstanceName = redisInstanceName;
});

// Регистрируем сервис кэширования
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// Логируем информацию о подключении
Log.Information("Redis кэширование настроено. Подключение: {Connection}, Префикс: {Prefix}",
  redisConnection.Replace("password=redisPassword", "password=***"),
  redisInstanceName);

builder.Services.AddScoped<UserRepository>();

builder.Services.AddSingleton<IEventBus, RabbitMqEventBus>();

// Регистрация зависимостей (DI)
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RoleService>();

// Настройка аутентификации (JWT)
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("Ключ JWT не настроен в 'Jwt:Key'.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Проверка куки для получения токена
                if (context.Request.Cookies.ContainsKey("AUTH_COOKIE"))
                {
                    context.Token = context.Request.Cookies["AUTH_COOKIE"];
                }
                return Task.CompletedTask;
            }
        };
    });

// Добавление контроллеров
builder.Services.AddControllers();

// Настройка CORS - будет централизованно обрабатываться на уровне Gateway
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGateway", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Настройка Swagger (документация API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Service API", Version = "v1" });

    // Добавляем поддержку авторизации в Swagger
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

// Сборка приложения
var app = builder.Build();

// Применение миграций автоматически при запуске
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        Log.Information("Применение миграций...");
        dbContext.Database.Migrate();
        Log.Information("Миграции успешно применены.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Ошибка при применении миграций");
    }
}

// Middleware для разработки
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Показывает детальную информацию об ошибках
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "api/user/swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api/user/swagger/v1/swagger.json", "User Service API");
        c.RoutePrefix = "api/user/swagger";
    });
}

// Middleware для логирования запросов
app.UseSerilogRequestLogging();

// Политика CORS - управляется Gateway
app.UseCors("AllowGateway");

// Маршрутизация и аутентификация
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Маршрутизация для контроллеров
app.MapControllers();

// Запуск приложения
try
{
    Log.Information("Запуск UserService");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение UserService завершилось неожиданно");
}
finally
{
    Log.CloseAndFlush();
}
