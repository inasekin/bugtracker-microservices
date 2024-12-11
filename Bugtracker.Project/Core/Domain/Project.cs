using System;
using System.Collections.Generic;

namespace BugTracker.Domain
{
	/// <summary>
	/// Класс сущности Проект.
	/// Основная единица на которую выдаются права и пользователи и производится конфигурация.
	/// Проекты могут входить друг в друга тем самым наследуя пользователей и настройки.
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
		public IEnumerable<ProjectUser> Users { get; private set; }

        // Задачи. Если микросервис, то задачи будем получать в репо Задачи
        // public IEnumerable<Guid> Issues { get; private set; }

        // TODO: Категории задач, которые можно создавать в проекте
        //public IEnumerable<IssueCategory> IssueCategories { get; private set; }

        /// <summary>
        /// Родительский проект
        /// </summary>
        public Project Parent { get; private set; }
		public Project ParentId { get; private set; }
	
        /// <summary>
        /// Техническое поле версии объекта
        /// </summary>
        public int Version { get; private set; }
    }
}
