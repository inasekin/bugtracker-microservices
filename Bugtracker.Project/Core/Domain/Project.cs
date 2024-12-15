using System;
using System.Collections.Generic;

namespace BugTracker.Domain
{
	/// <summary>
	/// сущность Проект.
	/// Основная единица на которую выдаются права и пользователи и производится конфигурация.
	/// Проекты могут входить друг в друга тем самым наследуя пользователей и настройки
	/// </summary>
    public class Project
    {
        /// <summary>
        /// Первичный ключ
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Строковый уникальный id для использования в url и внешних ссылок 
        /// </summary>
        public string SysId { get; private set; }

        /// <summary>
        /// Имя проекта для отображения
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Расширенное описание
        /// </summary>
        public string Description { get; private set; }

		// TODO: Насследовать пользователей с родительского проекта
		// public bool InheritUsers { get; private set; }

		// TODO: Публичный проект доступный всем пользователям
		// public bool Public { get; private set; }
	
        /// <summary>
        /// Пользователи проекта и роли в которых они участвуют
        /// </summary>
		public IEnumerable<ProjectUserRoles> UserRoles { get; private set; }

        /// <summary>
        /// Классификатор: категории задач, которые можно создавать в проекте
        /// </summary>
        public IEnumerable<ProjectIssueCategory> IssueCategories { get; private set; }

        /// <summary>
        /// Классификатор: Версии, которые есть у текущего проекта
        /// </summary>
        public IEnumerable<ProjectIssueCategory> IssueVersion { get; private set; }

        /// <summary>
        /// Классификатор: Типы задач, которые можно создавать/использовать в текущем проекте
        /// </summary>
        public IEnumerable<ProjectIssueType> IssueTypes { get; private set; }

        // Задачи. Если микросервис, то задачи будем получать в репо Задачи
        // public IEnumerable<Guid> Issues { get; private set; }

        /// <summary>
        /// Родительский проект
        /// </summary>
        /// <remarks>
        /// Проекты могут входить друг в друга и наследовать пользователей и права
        /// </remarks>
        public Project Parent { get; private set; }

        /// <summary>
        /// Guid родительского проекта
        /// </summary>
		public Guid ParentId { get; private set; }
	}
}
