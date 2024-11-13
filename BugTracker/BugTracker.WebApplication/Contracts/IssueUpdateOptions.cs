using BugTracker.Domain;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.WebApplication.Contracts
{
    public record IssueUpdateOptions(
        [Required][MaxLength(Issue.MAX_TOPIC_LENGTH)] string Topic,
        [MaxLength(Issue.MAX_DESCRIPTION_LENGTH)] string Description,
        IssueStatus Status,
        IssueCategory Category,
        DateTime StartDate,
        DateTime EndDate,
        int Readiness,
        string AffectedVersion,
        int TaskItemVersion);
    
}
