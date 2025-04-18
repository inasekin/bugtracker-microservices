using AutoMapper;
using IssueService.Api.Mappers;
using IssueService.DAL;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    var enumConverter = new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower);
    opts.JsonSerializerOptions.Converters.Add(enumConverter);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IssueRepository>();
//builder.Services.AddScoped<ProjectsEfDbInitializer>();

var connectionString = builder.Configuration.GetConnectionString("BugtrackerIssuesDb");
builder.Services.AddDbContext<ApplicationDbContext>(x =>
{
    //x.UseSqlite("Filename=bugtracker-projects.sqlite");
    x.UseNpgsql(connectionString);
    // x.UseSnakeCaseNamingConvention();
    // x.UseLazyLoadingProxies();
});

var mapperConfiguration = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<IssueMappingProfile>();
});
mapperConfiguration.AssertConfigurationIsValid();
builder.Services.AddSingleton<IMapper>(new Mapper(mapperConfiguration));

builder.Services.AddScoped<EfDbInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// Create db
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetService<EfDbInitializer>();
    dbInitializer.InitializeDb();
}

app.Run();
