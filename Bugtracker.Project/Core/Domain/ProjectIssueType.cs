using System;

namespace BugTracker.Domain
{
    /// <summary>
	/// Связь проекта с разрешенными типами задач
	/// </summary>
    public class ProjectIssueType
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
        /// Идентификатор типа Issue
        /// </summary>
        public Guid IssueTypeId { get; private set; }
    }
}
