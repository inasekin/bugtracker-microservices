using FileService.Api;
using FileService.Api.Extensions;
using FileService.DAL;
using FileService.DAL.Extensions;
using FileService.Domain.Extensions;
using Microsoft.AspNetCore.Http.Features;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"Среда выполнения: {environment}");

var appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

var services = builder.Services;

services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));

// Add Database Connection
var connectionString = appSettings?.ConnectionString
           ?? throw new InvalidOperationException("Строка подключения не найдена.");
services.AddDataAccessLayer(connectionString);

// Add services to the container.
services.AddDomainServices(builder.Configuration.GetSection(nameof(AppSettings)));

// Add automapper 
services.AddMapperProfiles();

services.AddControllers();
services.Configure<FormOptions>(x => {
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = long.MaxValue; // In case of multipart
});
 
// Настройка CORS
string[] allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"];
foreach (var allowedOrigin in allowedOrigins)
    Console.WriteLine(allowedOrigin);
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


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

FileDbInitializer.Initialize(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Показывает детальную информацию об ошибках
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "api/files/swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api/files/swagger/v1/swagger.json", "Files Service API");
        c.RoutePrefix = "api/files/swagger";
    });
}

app.UseAuthorization();

if (app.Environment.IsDevelopment())
    app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
else
    app.UseCors("NextJsPolicy");

app.MapControllers();

app.Run();
