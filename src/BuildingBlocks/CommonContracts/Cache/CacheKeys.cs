namespace CommonContracts.Cache
{
  /// <summary>
  /// Статический класс для стандартизации ключей кэша между сервисами
  /// </summary>
  public static class CacheKeys
  {
    /// <summary>
    /// Префикс для ключей кэша пользователей
    /// </summary>
    public const string UserPrefix = "user_";

    /// <summary>
    /// Префикс для ключей кэша проектов
    /// </summary>
    public const string ProjectPrefix = "project_";

    /// <summary>
    /// Префикс для ключей кэша комментариев
    /// </summary>
    public const string CommentPrefix = "comment_";

    /// <summary>
    /// Формирует ключ кэша для пользователя по ID
    /// </summary>
    public static string ForUser(Guid userId) => $"{UserPrefix}{userId}";

    /// <summary>
    /// Формирует ключ кэша для роли пользователя
    /// </summary>
    public static string ForUserRole(Guid userId) => $"{UserPrefix}role_{userId}";

    /// <summary>
    /// Формирует ключ кэша для пользователя по email
    /// </summary>
    public static string ForUserByEmail(string email) => $"{UserPrefix}email_{email}";

    /// <summary>
    /// Формирует ключ кэша для проекта
    /// </summary>
    public static string ForProject(Guid projectId) => $"{ProjectPrefix}{projectId}";

    /// <summary>
    /// Формирует ключ кэша для комментария
    /// </summary>
    public static string ForComment(Guid commentId) => $"{CommentPrefix}{commentId}";

    /// <summary>
    /// Формирует ключ кэша для комментариев по ID задачи
    /// </summary>
    public static string ForCommentsByIssue(Guid issueId) => $"{CommentPrefix}issue_{issueId}";
  }
}
