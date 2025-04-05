using System.ComponentModel;

namespace IssueService.Domain.Enums;

public enum IssueStatus
{
    [Description("Создано")]
    Todo,

    [Description("В работе")]
    InProgress, // Сериализуем в "in_progress" -> JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower)

    [Description("Решено")]
    Review,

    [Description("Закрыто")]
    Done
}
