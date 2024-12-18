using CSharpFunctionalExtensions;
using System.Runtime.CompilerServices;

namespace BugTracker.Domain
{
    // Категории задач, это распределение по модулям/сервисам/функционалу
    // Каждому человеку назначена категория
    public class ProjectIssueCategory
    {
	// Первичный ключ
	public Guid Id { get; private set; }

	// Имя категории
	public string Name { get; private set; }

	// Идентификатор проекта
	public Guid ProjectId { get; private set; }

	// Идентификатор пользователя, на которого назначена категория. Может быть null.
	public Guid UserId { get; private set; }
    }
}
