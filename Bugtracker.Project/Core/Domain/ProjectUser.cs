using System;
using System.Collections.Generic;

namespace BugTracker.Domain
{
    /// <summary>
	/// Пользователи проекта и роль которую они выполняют по отношению к проекту
	/// </summary>
    public class ProjectUser
    {
        /// <summary>
        /// Первичный ключ
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Идентификатор проекта
        /// </summary>
        public Guid ProjectId { get; private set; }

        /// <summary>
        /// Идентификатор пользователя.
        /// В таблице пользователи могут дублироваться, т.к. могут выполнять множество ролей
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Идентификатор роли
        /// </summary>
        public IEnumerable<Guid> RoleId { get; private set; }
    }
}