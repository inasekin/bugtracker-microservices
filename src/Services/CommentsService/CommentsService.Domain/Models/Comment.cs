namespace CommentsService.Domain.Models
{
    public class Comment : BaseEntity
    {
        public Guid IssueId { get; set; }
        public Guid AuthorId { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAtTime { get; set; }
        public DateTime? UpdatedAtTime { get; set; }
    }
}
