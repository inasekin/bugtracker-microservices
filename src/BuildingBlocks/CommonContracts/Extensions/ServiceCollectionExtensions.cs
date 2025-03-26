using Microsoft.Extensions.DependencyInjection;
using System;

namespace CommonContracts.Extensions
{
  /// <summary>
  /// Расширения для настройки сервисов
  /// </summary>
  public static class ServiceCollectionExtensions
  {
    /// <summary>
    /// Добавляет общие сервисы контрактов в DI контейнер
    /// </summary>
    public static IServiceCollection AddCommonContracts(this IServiceCollection services)
    {
      // Здесь можно зарегистрировать общие сервисы для всех микросервисов
      return services;
    }

    /// <summary>
    /// Настраивает сервисы для взаимодействия с сервисом пользователей
    /// </summary>
    public static IServiceCollection AddUserServiceClient(this IServiceCollection services, string userServiceUrl)
    {
      services.AddHttpClient("UserService", client =>
      {
        client.BaseAddress = new Uri(userServiceUrl);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
      });

      services.AddScoped<Auth.IUserServiceClient, Auth.UserServiceClient>();

      return services;
    }
  }
}
