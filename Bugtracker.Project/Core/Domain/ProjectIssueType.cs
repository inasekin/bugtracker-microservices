using System;

namespace BugTracker.Domain
{
    /// <summary>
	/// Связь проекта с разрешенными типами задач
	/// </summary>
    /// <remarks>
    /// В каждый проект могут входить свои типы задач, например: Задача, Предложение, Вопрос
    /// </remarks>
    public class ProjectIssueType
    {
        /// <summary>
        /// Первичный ключ
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Проект
        /// </summary>
        public Project Project { get; set; }

        /// <summary>
        /// Идентификатор типа задачи(Issue)
        /// </summary>
        /// <remarks>
        /// Тип ззадачи, например: Задача, Предложение, Вопрос
        /// TODO: Если примем решение, что тип задачи зашит в коде, как enum, то это просто поменять Guid IssueTypeId => enum IssueType
        /// </remarks>
        public string IssueType { get; set; }
    }
}
