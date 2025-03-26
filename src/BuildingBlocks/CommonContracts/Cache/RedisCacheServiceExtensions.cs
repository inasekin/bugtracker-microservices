using Microsoft.Extensions.DependencyInjection;
using System;

namespace CommonContracts.Cache
{
  /// <summary>
  /// Расширения для настройки Redis кэширования
  /// </summary>
  public static class RedisCacheServiceExtensions
  {
    /// <summary>
    /// Добавляет службу Redis кэширования в контейнер зависимостей
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="redisConnectionString">Строка подключения к Redis</param>
    /// <param name="instanceName">Имя экземпляра (префикс для ключей)</param>
    /// <returns>Коллекция сервисов с добавленным кэшированием</returns>
    public static IServiceCollection AddRedisCacheService(
      this IServiceCollection services,
      string redisConnectionString,
      string instanceName = "")
    {
      if (string.IsNullOrEmpty(redisConnectionString))
      {
        throw new ArgumentException("Строка подключения к Redis не может быть пустой", nameof(redisConnectionString));
      }

      // Настройка Redis
      services.AddStackExchangeRedisCache(options =>
      {
        options.Configuration = redisConnectionString;
        options.InstanceName = instanceName;
      });

      // Регистрация сервиса кэширования
      // Реализацию нужно будет добавить в каждый микросервис, так как она зависит от конкретных библиотек
      // services.AddSingleton<ICacheService, RedisCacheService>();

      return services;
    }
  }
}
