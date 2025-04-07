namespace FileService.Api.Contracts;

public sealed class FileInfoResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
