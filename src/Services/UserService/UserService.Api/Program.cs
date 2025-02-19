using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using UserService.Api.Services;
using UserService.DAL;

using System.Text;
using EventBus;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"Среда выполнения: {environment}");

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
        .LogTo(Console.WriteLine));
Console.WriteLine("Успешное подключение к базе данных.");

builder.Services.AddScoped<UserRepository>();

builder.Services.AddSingleton<IEventBus, RabbitMqEventBus>();

// Регистрация зависимостей (DI)
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<AuthService>();

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
            ValidateAudience = false
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

// Настройка CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("NextJsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Разрешение передачи куки
    });
});

// Настройка Swagger (документация API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Service API", Version = "v1" });
});

// Сборка приложения
var app = builder.Build();

// Применение миграций автоматически при запуске
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        Console.WriteLine("Применение миграций...");
        dbContext.Database.Migrate();
        Console.WriteLine("Миграции успешно применены.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при применении миграций: {ex.Message}");
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

// Политика CORS
app.UseCors("NextJsPolicy");

// Маршрутизация и аутентификация
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Маршрутизация для контроллеров
app.MapControllers();

// Запуск приложения
app.Run();
