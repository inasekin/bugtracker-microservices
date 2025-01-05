using CSharpFunctionalExtensions;
using System.Runtime.CompilerServices;

namespace BugTracker.Domain
{
    public class Project
    {
	// Первичный ключ
	public Guid Id { get; private set; }

	// Строковый уникальный id для использования в url и внешних ссылок
	public string SysId { get; private set; }

	// Имя проекта для отображения
        public string Name { get; private set; }

	// Расширенное описание
        public string Description { get; private set; }

	// TODO: Насследовать пользователей с родительского проекта
	// public bool InheritUsers { get; private set; }

	// TODO: Публичный проект доступный всем пользователям
	// public bool Public { get; private set; }
	
	public IEnumerable<ProjectUsers> Users { get; private set; }

	// Задачи. Если микросервис, то задачи будем получать в репо Задачи
	// public IEnumerable<Guid> Issues { get; private set; }

	// TODO: Категории задач, которые можно создавать в проекте
	//public IEnumerable<IssueCategory> IssueCategories { get; private set; }
	
	// Родительский проект
	public Project Parent { get; private set; }
	public Project ParentId { get; private set; }
	
        /// <summary>
        /// Техническое поле версии объекта
        /// </summary>
        public int Version { get; private set; }
    }
}
