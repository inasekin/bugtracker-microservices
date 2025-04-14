using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/gateway-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.WebHost.UseUrls("http://0.0.0.0:80");

var microservices = new[]
{
    new {
        Name = "UserService",
        Prefix = "user",
        Host = "userservice",
        Port = 80,
        SwaggerUrl = "http://userservice/api/user/swagger/v1/swagger.json"
    },
    new {
        Name = "ProjectService",
        Prefix = "project",
        Host = "projectservice",
        Port = 80,
        SwaggerUrl = "http://projectservice/api/project/swagger/v1/swagger.json"
    },
    new {
        Name = "CommentsService",
        Prefix = "comments",
        Host = "commentsservice",
        Port = 80,
        SwaggerUrl = "http://commentsservice/api/comments/swagger/v1/swagger.json"
    },
    new {
        Name = "FileService",
        Prefix = "files",
        Host = "fileservice",
        Port = 80,
        SwaggerUrl = "http://fileservice/api/files/swagger/v1/swagger.json"
    }
};

var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsASecretKeyWithEnoughLength12345";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true,
            ValidateLifetime = true
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Логируем заголовки для отладки
                Log.Debug("Заголовки запроса: {@Headers}", context.Request.Headers);

                // Проверка токена в заголовке Authorization
                string authHeader = context.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    string token = authHeader.Substring("Bearer ".Length).Trim();
                    context.Token = token;
                    Log.Debug("Найден токен в заголовке Authorization: {TokenPrefix}...", token.Substring(0, Math.Min(20, token.Length)));
                    return Task.CompletedTask;
                }

                // Проверка токена в куках
                if (context.Request.Cookies.ContainsKey("AUTH_COOKIE"))
                {
                    context.Token = context.Request.Cookies["AUTH_COOKIE"];
                    Log.Debug("Найден токен в куках");
                    return Task.CompletedTask;
                }

                Log.Debug("Токен не найден ни в заголовке, ни в куках");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Log.Warning("Ошибка аутентификации: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Log.Debug("Токен успешно валидирован");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddLogging(logging =>
{
    logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);
});

var routes = new List<object>();

foreach (var svc in microservices)
{
    routes.Add(new {
        Priority = 1,
        UpstreamPathTemplate = $"/api/{svc.Prefix}/{{everything}}",
        UpstreamHttpMethod = new[] { "Get","Post","Put","Delete","Patch","Options" },
        DownstreamPathTemplate = $"/api/{svc.Prefix}/{{everything}}",
        DownstreamScheme = "http",
        DownstreamHostAndPorts = new[] {
            new { Host = svc.Host, Port = svc.Port }
        },
        AuthenticationOptions = new {
            AuthenticationProviderKey = "Bearer",
            AllowedScopes = new string[] { }
        },
        RateLimitOptions = new {
            ClientWhitelist = new string[] { },
            EnableRateLimiting = true,
            Period = "1s",
            PeriodTimespan = 1,
            Limit = 10
        },
        FileCacheOptions = new {
            TtlSeconds = 15
        }
    });

    routes.Add(new {
        Priority = 90,
        UpstreamPathTemplate = $"/api/{svc.Prefix}/swagger/v1/swagger.json",
        UpstreamHttpMethod = new[] { "Get" },
        DownstreamPathTemplate = $"/api/{svc.Prefix}/swagger/v1/swagger.json",
        DownstreamScheme = "http",
        DownstreamHostAndPorts = new[] {
            new { Host = svc.Host, Port = svc.Port }
        }
    });

    routes.Add(new {
        Priority = 100,
        UpstreamPathTemplate = $"/api/{svc.Prefix}/swagger/{{everything}}",
        UpstreamHttpMethod = new[] { "Get" },
        DownstreamPathTemplate = $"/api/{svc.Prefix}/swagger/{{everything}}",
        DownstreamScheme = "http",
        DownstreamHostAndPorts = new[] {
            new { Host = svc.Host, Port = svc.Port }
        }
    });
}

var publicRoutes = new List<object>
{
  new {
    Priority = 500,
    UpstreamPathTemplate = "/api/user/auth/register",
    UpstreamHttpMethod = new[] { "Post", "Options" },
    DownstreamPathTemplate = "/api/user/auth/register",
    DownstreamScheme = "http",
    DownstreamHostAndPorts = new[] {
      new { Host = "userservice", Port = 80 }
    }
  },
  // Вход
  new {
    Priority = 500,
    UpstreamPathTemplate = "/api/user/auth/login",
    UpstreamHttpMethod = new[] { "Post", "Options" },
    DownstreamPathTemplate = "/api/user/auth/login",
    DownstreamScheme = "http",
    DownstreamHostAndPorts = new[] {
      new { Host = "userservice", Port = 80 }
    }
  },
  // Проверка текущего пользователя
  new {
    Priority = 500,
    UpstreamPathTemplate = "/api/user/auth/current",
    UpstreamHttpMethod = new[] { "Get", "Options" },
    DownstreamPathTemplate = "/api/user/auth/current",
    DownstreamScheme = "http",
    DownstreamHostAndPorts = new[] {
      new { Host = "userservice", Port = 80 }
    }
  },
  // Swagger UI и документация
  new {
    Priority = 500,
    UpstreamPathTemplate = "/swagger/{everything}",
    UpstreamHttpMethod = new[] { "Get" },
    DownstreamPathTemplate = "/swagger/{everything}",
    DownstreamScheme = "http",
    DownstreamHostAndPorts = new[] {
      new { Host = "gatewayservice", Port = 80 }
    }
  }
};

routes.AddRange(publicRoutes);

var ocelotConfig = new {
    Routes = routes,
    GlobalConfiguration = new {
        BaseUrl = "http://localhost:5010",
        RequestIdKey = "RequestId",
        DownstreamScheme = "http",
        RateLimitOptions = new {
            DisableRateLimitHeaders = false,
            QuotaExceededMessage = "Слишком много запросов, пожалуйста, попробуйте позже",
            HttpStatusCode = 429
        }
    }
};

var jsonString = JsonConvert.SerializeObject(ocelotConfig, Formatting.Indented);
Log.Information("=== КОНФИГУРАЦИЯ МАРШРУТОВ ===");
Log.Information(jsonString);

builder.Configuration.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(jsonString)));

builder.Services.AddOcelot(builder.Configuration)
    .AddCacheManager(x => {
        x.WithDictionaryHandle();
    });

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                    ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("NextJsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Gateway Single Swagger",
        Version = "v1"
    });
    opt.CustomSchemaIds(type => type.FullName);

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger"; // => /swagger/index.html

        // Подключаем swagger.json каждого сервиса
        foreach (var svc in microservices)
        {
            c.SwaggerEndpoint(
                $"/api/{svc.Prefix}/swagger/v1/swagger.json",
                svc.Name
            );
        }
    });
}

app.UseCors("NextJsPolicy");

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();
