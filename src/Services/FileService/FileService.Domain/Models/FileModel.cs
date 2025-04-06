namespace FileService.Domain.Models;

public class FileModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public FileModel() { }
    public FileModel(string name)
    {
        Name = name;
    }
}
