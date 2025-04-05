using System.ComponentModel;

using IssueService.Domain.Enums;

public enum IssueType
{
    [Description("Задача")]
    Task,

    [Description("Ошибка")]
    Bug,

    [Description("Предложение")]
    Feature
};
