using FileService.Domain.Models;

namespace FileService.Domain.Services;
public interface IFileService
{
    Task<FileModel> SaveFileAsync(string fileName, Stream stream, CancellationToken cancellationToken);
    Task<FileModel?> GetFileAsync(Guid guid, CancellationToken cancellationToken);
    Task<byte[]?> GetFileDataAsync(Guid guid, CancellationToken cancellationToken);
    Task DeleteFileAsync(Guid guid, CancellationToken cancellationToken);
    Task CommitChangesAsync(CancellationToken cancellationToken);
}
