namespace CommentsService.Api.Models
{
    public class CreateCommentRequest
    {
        public Guid IssueId { get; set; }
        public Guid AuthorId { get; set; }
        public string? Content { get; set; }
    }
}
