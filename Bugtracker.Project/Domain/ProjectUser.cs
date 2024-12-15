using CSharpFunctionalExtensions;
using System.Runtime.CompilerServices;

namespace BugTracker.Domain
{
    // Пользователи проекта и роль которую они выполняют по отношению к проекту
    public class ProjectUser
    {
	// Первичный ключ
	public Guid Id { get; private set; }

	// Идентификатор проекта
	public Guid ProjectId { get; private set; }

	// Идентификатор пользователя. 
	// В таблице пользователи могут дублироваться, т.к. могут выполнять множество ролей
	public Guid UserId { get; private set; }

	// Идентификатор роли.
	public IEnumerable<Guid> RoleId { get; private set; }
    }
}
