using System;

namespace CommonContracts.Events
{
  /// <summary>
  /// Событие создания пользователя, которое публикуется в шину событий
  /// </summary>
  public class UserCreatedEvent
  {
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Guest";
    public DateTime CreatedAt { get; set; }
  }

  /// <summary>
  /// Событие обновления роли пользователя
  /// </summary>
  public class UserRoleChangedEvent
  {
    public Guid Id { get; set; }
    public string OldRole { get; set; } = string.Empty;
    public string NewRole { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
  }
}
