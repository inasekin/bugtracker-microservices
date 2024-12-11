using System;

namespace BugTracker.Domain
{
    /// <summary>
    /// Категории задач, это распределение по модулям/сервисам/функционалу
    /// Каждому человеку назначена категория
    /// </summary>
    public class ProjectIssueCategory
    {
        /// <summary>
        /// Первичный ключ
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Имя категории
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Идентификатор проекта
        /// </summary>
        public Guid ProjectId { get; private set; }

        /// <summary>
        /// Идентификатор пользователя, на которого назначена категория. Может быть null.
        /// </summary>
        public Guid UserId { get; private set; }
    }
}
