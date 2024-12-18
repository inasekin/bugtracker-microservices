using System;

namespace BugTracker.Domain
{
    /// <summary>
	/// Связь пользователя проекта и роли которую он выполняет по отношению к проекту
	/// </summary>
    /// <remarks>
    /// В одном проекте пользователь может выполнять разные роли, например: Тестироващик, Руководитель проекта, Разработчик.
    /// Каждая роль обладает определенным набором прав
    /// </remarks>
    public class ProjectUserRoles : BaseEntity
    {
        /// <summary>
        /// Проект
        /// </summary>
        public Project Project { get; set; }

        /// <summary>
        /// Идентификатор пользователя.
        /// В таблице пользователи могут дублироваться, т.к. могут выполнять множество ролей
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Идентификатор роли
        /// </summary>
        public string RoleId { get; set; }
    }
}