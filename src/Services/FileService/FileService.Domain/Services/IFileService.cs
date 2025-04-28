using FileService.Domain.Models;

namespace FileService.Domain.Services;
public interface IFileService
{
    Task<FileModel> SaveFileAsync(string fileName, Stream stream, CancellationToken cancellationToken);
    IEnumerable<FileModel> GetAllFileInfo(int take, int skip);
    Task<FileModel?> GetFileInfoAsync(Guid guid, CancellationToken cancellationToken);
    Task DeleteFileAsync(Guid guid, CancellationToken cancellationToken);
    Task CommitChangesAsync(CancellationToken cancellationToken);
}
