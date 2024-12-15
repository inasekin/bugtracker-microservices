using CSharpFunctionalExtensions;
using System.Runtime.CompilerServices;

namespace BugTracker.Domain
{
    // Связь проекта с разрешенными типами задач
    public class ProjectIssueType
    {
	// Первичный ключ
	public Guid Id { get; private set; }

	// Идентификатор проекта
	public Guid ProjectId { get; private set; }

	// Идентификатор типа Issue
	public Guid IssueTypeId { get; private set; }
    }
}
