using System.ComponentModel;

namespace IssueService.Domain.Enums;

public enum IssuePriority
{
    [Description("Низкий")]
    Low,

    [Description("Нормальный")]
    Medium,

    [Description("Высокий")]
    High,

    [Description("Блокирующий")]
    Critical
}
