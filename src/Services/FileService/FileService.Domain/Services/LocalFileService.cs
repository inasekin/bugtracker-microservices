using AutoMapper;
using FileService.DAL.Entities;
using FileService.DAL.Repositories;
using FileService.Domain.Models;
using Microsoft.Extensions.Options;

namespace FileService.Domain.Services;

public class LocalFileService(
  IRepository<FileEntity> repository,
  IUnitOfWork unitOfWork,
  IOptions<FileStorageSettings> options,
  IMapper mapper
  )
  : IFileService
{
    private readonly IRepository<FileEntity> _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly FileStorageSettings _settings = options.Value;

    public Task CommitChangesAsync(CancellationToken cancellationToken)
    {
        return _unitOfWork.SaveChanges(cancellationToken);
    }

    public Task DeleteFileAsync(Guid guid, CancellationToken cancellationToken)
    {
        return _repository.RemoveAsync(guid, cancellationToken);
    }

    public IEnumerable<FileModel> GetAllFileInfo(int take, int skip)
    {
        var entities = _repository.GetAll(take, skip);

        if (entities == null || !entities.Any())
            return [];

        return entities.Select(e =>
        {
            var m = _mapper.Map<FileModel>(e);
            m.Path = Path.Combine(_settings.FolderPath, m.Name);
            return m;
        });
    }

    public async Task<FileModel?> GetFileInfoAsync(Guid guid, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(guid, cancellationToken);
        if (entity == null)
            return null;

        var model = _mapper.Map<FileModel>(entity);
        model.Path = Path.Combine(_settings.FolderPath, model.Name);

        return model;
    }

    public async Task<FileModel> SaveFileAsync(string fileName, Stream stream, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(fileName);
        var dirPath = _settings.FolderPath;

        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        if (File.Exists(Path.Combine(dirPath, fileName)))
            fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), "-", Guid.NewGuid(), extension);

        var filePath = Path.Combine(dirPath, fileName);

        using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream, cancellationToken);

        var model = new FileModel(fileName);
        var entity = await _repository.AddAsync(_mapper.Map<FileEntity>(model), cancellationToken);
        return _mapper.Map<FileModel>(entity);
    }
}
