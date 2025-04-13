using IssueService.Domain.Enums;

namespace IssueService.Api.Contracts;

public class IssueRequest
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;

    public IssueStatus Status { get; set; } = IssueStatus.Todo;
    public IssuePriority Priority { get; set; } = IssuePriority.Medium;
    public IssueType Type { get; set; } = IssueType.Task;

    public Guid AuthorId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AssigneeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }
};
