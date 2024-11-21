using BugTracker.WebApplication.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BugTracker.WebApplication.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"Среда выполнения: {environment}");

// Подключение к базе данных
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(conn))
{
    throw new InvalidOperationException("Строка подключения 'DefaultConnection' не найдена.");
}

// Регистрация DbContext
builder.Services.AddDbContext<BugTrackerDbContext>(options =>
    options.UseNpgsql(conn));
Console.WriteLine("Успешное подключение к базе данных.");

// Регистрация зависимостей (DI)
builder.Services.AddScoped<UserService>();
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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BugTracker API", Version = "v1" });
});

// Сборка приложения
var app = builder.Build();

// Middleware для разработки
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Показывает детальную информацию об ошибках
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Политика CORS
app.UseCors("NextJsPolicy");

// Настройка статических файлов
var staticFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
if (Directory.Exists(staticFilesPath))
{
    app.UseStaticFiles();
}
else
{
    Console.WriteLine("Предупреждение: Директория статических файлов 'wwwroot' не найдена.");
}

// Маршрутизация и аутентификация
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Маршрутизация для контроллеров
app.MapControllers();

// Резервный маршрут для SPA (одностраничное приложение)
app.MapFallbackToFile("/index.html").AllowAnonymous();

// Запуск приложения
app.Run();
