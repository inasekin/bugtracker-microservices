using BugTracker.Domain;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.WebApplication.Contracts
{
    public record TaskItemCreationOptions(
        [Required][MaxLength(TaskItem.MAX_TOPIC_LENGTH)] string Topic,
        [MaxLength(TaskItem.MAX_DESCRIPTION_LENGTH)] string Description,
        TaskItemStatus Status,
        TaskItemCategory Category,
        DateTime StartDate,
        DateTime EndDate,
        int Readiness,
        string AffectedVersion);
    
}
