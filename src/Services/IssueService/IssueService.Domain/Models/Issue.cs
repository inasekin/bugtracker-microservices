using IssueService.Domain.Enums;

namespace IssueService.Domain.Models;
public class Issue
{
  public Guid Id { get; set; }
  public string Name { get; set; } = null!;
  public string Description { get; set; } = null!;
  public Guid AuthorId { get; set; }
  public Guid ProjectId { get; set; }
  public Guid AssignedTo { get; set; }
  public DateTime Created { get; set; }
  public DateTime Updated { get; set; }
  public Status Status { get; set; } = Status.Created;
}
