using FileService.Api;
using FileService.Api.Extensions;
using FileService.DAL.Extensions;
using FileService.Domain.Extensions;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"Среда выполнения: {environment}");

var appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

// Подключение к базе данных
var connectionString = appSettings?.ConnectionString
           ?? throw new InvalidOperationException("Строка подключения не найдена.");

var services = builder.Services;

services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));

// Регистрация DbContext
services.AddDataAccessLayer(connectionString);

// Add services to the container.
services.AddDomainServices(builder.Configuration.GetSection(nameof(AppSettings)));

// Add automapper 
services.AddMapperProfiles();

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
