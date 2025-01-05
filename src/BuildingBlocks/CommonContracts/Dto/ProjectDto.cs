namespace CommonContracts.Dto
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }  // userId
    }
}