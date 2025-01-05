using System;

namespace BugTracker.Domain
{
	/// <summary>
    /// Базовый класс для моделей
	/// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Первичный ключ
        /// </summary>
        public Guid Id { get; set; }
	}
}
