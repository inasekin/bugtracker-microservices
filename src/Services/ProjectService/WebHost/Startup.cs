using AutoMapper;
using Bugtracker.DataAccess;
using Bugtracker.WebHost.Mapping;
using BugTracker.DataAccess;
using BugTracker.DataAccess.Repositories;
using BugTracker.Domain;
using DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace PromoCodeFactory.WebHost
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddMvcOptions(x => 
                x.SuppressAsyncSuffixInActionNames = false);

            // Настройка CORS
            string[] allowedOrigins = Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" };
            services.AddCors(options =>
            {
              options.AddPolicy("NextJsPolicy", policy =>
              {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials(); // Разрешение передачи куки
              });
            });

            services.AddScoped(typeof(IUnitOfWork), typeof(ProjectRepositoryUnitOfWork));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IRepository<Project>), typeof(ProjectRepository));

            services.AddScoped<ProjectsEfDbInitializer>();
            
            services.AddDbContext<DataContext>(x =>
            {
                x.UseSqlite("Filename=bugtracker-projects.sqlite");
                //x.UseNpgsql(Configuration.GetConnectionString("PromoCodeFactoryDb"));
                //    x.UseSnakeCaseNamingConvention();
                //    x.UseLazyLoadingProxies();
            });

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProjectMappingProfile>();
            });
            mapperConfiguration.AssertConfigurationIsValid();
            services.AddSingleton<IMapper>(new Mapper(mapperConfiguration));

            services.AddOpenApiDocument(options =>
             {
                options.Title = "PromoCode Factory API Doc";
                options.Version = "1.0";
            });

            // Добавление Swagger с кастомным маршрутом
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ProjectsEfDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseOpenApi();

            // Настройка Swagger с кастомными маршрутами
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api/project/swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/project/swagger/v1/swagger.json", "Project Service API");
                c.RoutePrefix = "api/project/swagger";
            });

            //app.UseHttpsRedirection();

            if(env.IsDevelopment())
              app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            else
              app.UseCors("NextJsPolicy");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            dbInitializer.InitializeDb();
        }
    }
}
