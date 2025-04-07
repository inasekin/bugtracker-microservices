using FileService.Domain;

namespace FileService.Api;

public sealed class AppSettings : FileStorageSettings
{
    public string ConnectionString { get; set; } = null!;
}
