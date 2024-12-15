using System;

namespace BugTracker.Domain
{
    /// <summary>
	/// Классификатор версия проекта
	/// </summary>
    /// <remarks>Версии проекта это просто атрибут список тегов для удобства: "1.0", "1.2", "2.0".
    /// Обычно когда создается задача присваиваются версии в которых нужно исправить ошибку,
    /// кроме того можно отображать ошибки присущие только определенной версии, либо перебрасывать задачи пожелания на следующую версию</remarks>
    public class ProjectVersion
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
        /// Название версии продукта/проекта
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание версии продукта/проекта
        /// </summary>
        public string Description { get; set; }

        ///// <summary>
        ///// Статус: Открыт, заблокирован, закрыт
        ///// </summary>
        //public string Status { get; set; }

        ///// <summary>
        ///// Начало этапа
        ///// </summary>
        //public DateTime StartDate { get; set; }

    }
}
