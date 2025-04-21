using IssueService.Domain.Enums;

namespace IssueService.Domain.Models;

public class Issue
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;

    public IssueStatus Status { get; set; } = IssueStatus.Todo;
    public IssuePriority Priority { get; set; } = IssuePriority.Medium;
    public IssueType Type { get; set; } = IssueType.Feature;

    public Guid AuthorId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AssigneeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public List<FileInfo> Files { get; set; } = null!;
};
