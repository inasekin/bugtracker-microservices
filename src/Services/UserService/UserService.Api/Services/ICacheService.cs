using System;
using System.Threading.Tasks;

namespace UserService.Api.Services
{
    /// <summary>
    /// Интерфейс сервиса кэширования
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Получает значение из кэша или создает его, если не существует
        /// </summary>
        /// <typeparam name="T">Тип кэшируемого значения</typeparam>
        /// <param name="key">Ключ кэша</param>
        /// <param name="factory">Функция для создания значения, если оно отсутствует в кэше</param>
        /// <param name="expiration">Время жизни значения в кэше (опционально)</param>
        /// <returns>Значение из кэша или созданное функцией factory</returns>
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);

        /// <summary>
        /// Получает значение из кэша
        /// </summary>
        /// <typeparam name="T">Тип кэшируемого значения</typeparam>
        /// <param name="key">Ключ кэша</param>
        /// <returns>Значение из кэша или default(T), если значение отсутствует</returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Сохраняет значение в кэше
        /// </summary>
        /// <typeparam name="T">Тип кэшируемого значения</typeparam>
        /// <param name="key">Ключ кэша</param>
        /// <param name="value">Значение для кэширования</param>
        /// <param name="expiration">Время жизни значения в кэше (опционально)</param>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Удаляет значение из кэша
        /// </summary>
        /// <param name="key">Ключ кэша</param>
        Task RemoveAsync(string key);

        /// <summary>
        /// Проверяет, существует ли значение в кэше
        /// </summary>
        /// <param name="key">Ключ кэша</param>
        /// <returns>true, если значение существует, иначе false</returns>
        Task<bool> ExistsAsync(string key);
    }
}
