using System;
using System.Collections.Generic;

namespace BugTracker.Domain
{
	/// <summary>
	/// сущность Проект.
	/// Основная единица на которую выдаются права и пользователи и производится конфигурация.
	/// Проекты могут входить друг в друга тем самым наследуя пользователей и настройки
	/// </summary>
    public class Project : BaseEntity
    {
        /// <summary>
        /// Имя проекта для отображения
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Расширенное описание
        /// </summary>
        public string Description { get; set; }

		// TODO: Насследовать пользователей с родительского проекта
		// public bool InheritUsers { get; private set; }

		// TODO: Публичный проект доступный всем пользователям
		// public bool Public { get; private set; }
	
        /// <summary>
        /// Пользователи проекта и роли в которых они участвуют
        /// </summary>
		public List<ProjectUserRoles> UserRoles { get; set; }

        /// <summary>
        /// Классификатор: категории задач, которые можно создавать в проекте
        /// </summary>
        public List<ProjectIssueCategory> IssueCategories { get; set; }

        /// <summary>
        /// Классификатор: Версии, которые есть у текущего проекта
        /// </summary>
        public List<ProjectVersion> Versions { get; set; }

        /// <summary>
        /// Классификатор: Типы задач, которые можно создавать/использовать в текущем проекте
        /// </summary>
        public List<ProjectIssueType> IssueTypes { get; set; }

        // Задачи. Если микросервис, то задачи будем получать в репо Задачи
        // public IEnumerable<Guid> Issues { get; private set; }

        /// <summary>
        /// Родительский проект
        /// </summary>
        /// <remarks>
        /// Проекты могут входить друг в друга и наследовать пользователей и права
        /// </remarks>
		public Guid? ParentProjectId { get; set; }
	}
}
