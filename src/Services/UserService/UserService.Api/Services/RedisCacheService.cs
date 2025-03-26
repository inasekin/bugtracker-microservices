using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UserService.Api.Services
{
    /// <summary>
    /// Реализация сервиса кэширования на основе Redis
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(10);
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisCacheService(
            IDistributedCache cache,
            ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
        }

        /// <summary>
        /// Получает значение из кэша или создает его, если не существует
        /// </summary>
        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            // Проверяем наличие в кэше
            T cachedValue = await GetAsync<T>(key);
            if (cachedValue != null)
            {
                _logger.LogDebug("Получены данные из кэша: {Key}", key);
                return cachedValue;
            }

            // Если нет в кэше, создаем
            T value = await factory();

            // Если null, не кэшируем
            if (value != null)
            {
                await SetAsync(key, value, expiration);
                _logger.LogDebug("Данные добавлены в кэш: {Key}", key);
            }

            return value;
        }

        /// <summary>
        /// Получает значение из кэша
        /// </summary>
        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                // Получаем из Redis
                byte[] cachedBytes = await _cache.GetAsync(key);
                if (cachedBytes == null || cachedBytes.Length == 0)
                {
                    return default;
                }

                // Десериализуем
                string cachedJson = Encoding.UTF8.GetString(cachedBytes);
                return JsonSerializer.Deserialize<T>(cachedJson, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении данных из кэша: {Key}", key);
                return default;
            }
        }

        /// <summary>
        /// Сохраняет значение в кэше
        /// </summary>
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                // Сериализуем объект
                string jsonValue = JsonSerializer.Serialize(value, _jsonOptions);
                byte[] bytes = Encoding.UTF8.GetBytes(jsonValue);

                // Настраиваем параметры кэширования
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
                };

                // Сохраняем в Redis
                await _cache.SetAsync(key, bytes, options);
                _logger.LogDebug("Данные сохранены в кэше: {Key} (срок: {Expiration})",
                    key, expiration ?? _defaultExpiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении данных в кэше: {Key}", key);
            }
        }

        /// <summary>
        /// Удаляет значение из кэша
        /// </summary>
        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
                _logger.LogDebug("Данные удалены из кэша: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении данных из кэша: {Key}", key);
            }
        }

        /// <summary>
        /// Проверяет существование записи в кэше
        /// </summary>
        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                byte[] cachedBytes = await _cache.GetAsync(key);
                return cachedBytes != null && cachedBytes.Length > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке существования данных в кэше: {Key}", key);
                return false;
            }
        }
    }
}
