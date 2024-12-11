using System;

namespace BugTracker.Domain
{
    /// <summary>
	/// Связь проекта с разрешенными типами задач
	/// </summary>
    public class ProjectVersion
    {
        /// <summary>
        /// Первичный ключ
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Название версии продукта/проекта
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Описание версии продукта/проекта
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Идентификатор проекта. У каждого проекта свои версии
        /// </summary>
        public Guid ProjectId { get; private set; }
    }
}
