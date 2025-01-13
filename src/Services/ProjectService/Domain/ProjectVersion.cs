using CSharpFunctionalExtensions;
using System.Runtime.CompilerServices;

namespace BugTracker.Domain
{
    // Связь проекта с разрешенными типами задач
    public class ProjectVersion
    {
	// Первичный ключ
	public Guid Id { get; private set; }

	// Название версии продукта/проекта
	public string Name { get; private set; }

	// Описание версии продукта/проекта
	public string Description { get; private set; }

	// Идентификатор проекта. У каждого проекта свои версии
	public Guid ProjectId { get; private set; }
    }
}
