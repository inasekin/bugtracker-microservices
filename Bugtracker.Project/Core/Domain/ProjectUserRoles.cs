using System;
using System.Collections.Generic;

namespace BugTracker.Domain
{
    /// <summary>
	/// Связь пользователя проекта и роли которую он выполняет по отношению к проекту
	/// </summary>
    /// <remarks>
    /// В одном проекте пользователь может выполнять разные роли, например: Тестироващик, Руководитель проекта, Разработчик.
    /// Каждая роль обладает определенным набором прав
    /// </remarks>
    public class ProjectUserRoles
    {
        /// <summary>
        /// Первичный ключ
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор проекта
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Идентификатор пользователя.
        /// В таблице пользователи могут дублироваться, т.к. могут выполнять множество ролей
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Идентификатор роли
        /// </summary>
        public Guid RoleId { get; set; }
    }
}