namespace UserService.Domain.Models
{
  /// <summary>
  /// Роли пользователей в системе
  /// </summary>
  public enum UserRole
  {
    /// <summary>
    /// Гость - ограниченный доступ, может просматривать назначенные задачи и проекты
    /// </summary>
    Guest = 0,

    /// <summary>
    /// Менеджер - может создавать и управлять проектами, назначать задачи
    /// </summary>
    Manager = 1,

    /// <summary>
    /// Администратор - полный доступ к системе
    /// </summary>
    Admin = 2
  }
}
