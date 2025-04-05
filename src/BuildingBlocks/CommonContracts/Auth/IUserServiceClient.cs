using System;
using System.Threading.Tasks;

namespace CommonContracts.Auth
{
  /// <summary>
  /// Интерфейс клиента для взаимодействия с сервисом пользователей
  /// </summary>
  public interface IUserServiceClient
  {
    /// <summary>
    /// Проверяет токен JWT и возвращает информацию о пользователе
    /// </summary>
    Task<JwtValidationResponse> ValidateTokenAsync(string token);

    /// <summary>
    /// Проверяет, имеет ли пользователь указанную роль
    /// </summary>
    Task<bool> HasRoleAsync(Guid userId, string role);
  }
}
