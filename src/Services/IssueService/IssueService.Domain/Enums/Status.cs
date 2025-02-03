using System.ComponentModel;

namespace IssueService.Domain.Enums;
public enum Status
{
  [Description("Создано")]
  Created,
  [Description("В работе")]
  AtWork,
  [Description("Решено")]
  Solved,
  [Description("Закрыто")]
  Closed
}
