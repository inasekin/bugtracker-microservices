# Отладка

Для отладки запускаем сервис на порту 5000 прописанном в под отладчиком
launchSettings.json 
В этом случае все кросдоменные запросы разрешены (IsDevelopement)

В случае release запускаем
make services-up
перестариваются все модули, в этом случае 
в startup.cs разрешены политики NextJsPolice ()
```
            if(env.IsDevelopment())
              app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            else
              app.UseCors("NextJsPolicy");
```

По умолчанию разрешен домен http://localhost:3000
на котором запускается ui сервис в отладке через команду "npm run dev"

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

Далее в frontend\.env файл, либо создать, либо исправить	
VITE_API_URL=http://localhost:5010
это адрес сервера на котором запущен back proxy(Ocelot) Gateway Service
бэк запросы идут на него, а не через localhost:3000 proxy,
поэтому cors тоже нужны

для отладки frontend/projects с локальным сервером в файле 
frontend\src\components\project\data\server-url.ts 
поменять адрес сервера бэка. отладочный 5000, а в docker 5010

Ну и возможно какие-то службы придётся потушить из docker чтобы не было конфликта адресов.

