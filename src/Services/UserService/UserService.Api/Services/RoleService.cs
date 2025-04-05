using Microsoft.Extensions.Caching.Memory;
using UserService.Domain.Models;

namespace UserService.Api.Services
{
    public class RoleService
    {
        private readonly UserManagementService _userManagementService;
        private readonly ICacheService _cache;
        private readonly ILogger<RoleService> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        public RoleService(
          UserManagementService userManagementService,
          ICacheService cache,
          ILogger<RoleService> logger)
        {
          _userManagementService = userManagementService;
          _cache = cache;
          _logger = logger;
        }

        /// <summary>
        /// Повышает роль пользователя с Guest до Manager
        /// </summary>
        public async Task<(bool Success, string Message)> UpgradeUserToManagerAsync(Guid userId)
        {
          // Получаем пользователя
          var user = await _userManagementService.GetUserByIdAsync(userId);
          if (user == null)
          {
            _logger.LogWarning("Не удалось повысить роль - пользователь {UserId} не найден", userId);
            return (false, "Пользователь не найден");
          }

          // Проверяем, является ли пользователь уже Менеджером или выше
          if (user.Role >= UserRole.Manager)
          {
            _logger.LogInformation("Пользователь {UserId} уже имеет роль Менеджера или выше", userId);
            return (true, "Пользователь уже имеет роль Менеджера или выше");
          }

          // Обновляем роль пользователя до Менеджера
          user.Role = UserRole.Manager;
          user.UpdatedAt = DateTime.UtcNow;

          await _userManagementService.UpdateUserAsync(user);

          // Инвалидируем кэш
          await _cache.RemoveAsync($"user_{userId}");
          await _cache.RemoveAsync($"user_role_{userId}");

          _logger.LogInformation("Пользователь {UserId} повышен с роли Гость до роли Менеджер", userId);
          return (true, "Пользователь успешно повышен до роли Менеджера");
        }

        /// <summary>
        /// Административная конечная точка для прямой установки роли пользователя
        /// </summary>
        public async Task<(bool Success, string Message)> SetUserRoleAsync(Guid adminId, Guid userId, UserRole newRole)
        {
            // Получаем администратора, чтобы проверить его права
            var adminUser = await _userManagementService.GetUserByIdAsync(adminId);
            if (adminUser == null)
            {
                _logger.LogWarning("Не удалось установить роль - администратор {AdminId} не найден", adminId);
                return (false, "Администратор не найден");
            }

            // Проверяем, является ли пользователь администратором
            if (adminUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("Не удалось установить роль - пользователь {UserId} не является администратором", adminId);
                return (false, "Только администраторы могут устанавливать роли пользователей");
            }

            // Получаем целевого пользователя
            var user = await _userManagementService.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Не удалось установить роль - целевой пользователь {UserId} не найден", userId);
                return (false, "Целевой пользователь не найден");
            }

            user.Role = newRole;
            user.UpdatedAt = DateTime.UtcNow;

            await _userManagementService.UpdateUserAsync(user);

            // Инвалидируем кэш с использованием Redis
            await _cache.RemoveAsync($"user_{userId}");
            await _cache.RemoveAsync($"user_role_{userId}");

            _logger.LogInformation("Администратор {AdminId} установил пользователю {UserId} роль {Role}", adminId, userId, newRole);
            return (true, $"Роль пользователя успешно обновлена до {newRole}");
        }

        /// <summary>
        /// Получает роль пользователя с кэшированием в Redis
        /// </summary>
        public async Task<(bool Success, UserRole Role)> GetUserRoleAsync(Guid userId)
        {
          // Используем Redis кэш
          string cacheKey = $"user_role_{userId}";

          // Изменяем подход к кэшированию, чтобы вернуть кортеж с флагом успеха и ролью
          var cachedRole = await _cache.GetAsync<UserRole?>(cacheKey);
          if (cachedRole.HasValue)
          {
            _logger.LogDebug("Роль пользователя {UserId} получена из кэша", userId);
            return (true, cachedRole.Value);
          }

          // Получаем из базы данных
          var user = await _userManagementService.GetUserByIdAsync(userId);
          if (user == null)
          {
            _logger.LogWarning("Не удалось получить роль - пользователь {UserId} не найден", userId);
            return (false, UserRole.Guest); // Возвращаем Guest как значение по умолчанию
          }

          // Кэшируем результат
          await _cache.SetAsync(cacheKey, user.Role, _cacheDuration);
          _logger.LogDebug("Роль пользователя {UserId} получена из базы данных и кэширована", userId);

          return (true, user.Role);
        }
    }
}
