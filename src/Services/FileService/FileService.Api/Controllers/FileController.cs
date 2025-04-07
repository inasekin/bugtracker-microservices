using AutoMapper;
using FileService.Api.Contracts;
using FileService.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController(IFileService fileService, IMapper mapper) : ControllerBase
{
    private readonly IFileService _fileService = fileService;
    private readonly IMapper _mapper = mapper;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FileInfoResponse>> GetFileInfo(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            return BadRequest("Id is empty");

        var file = await _fileService.GetFileAsync(id, cancellationToken);
        if (file == null)
            return NotFound();

        return Ok(_mapper.Map<FileInfoResponse>(file));
    }

    [HttpPost]
    public async Task<ActionResult<FileInfoResponse>> AddFile(IFormFile file, [FromQuery]string? fileName, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        var extension = Path.GetExtension(file.FileName);
        fileName = string.IsNullOrWhiteSpace(fileName) ? file.FileName : $"{fileName}{extension}";

        using var stream = new MemoryStream();

        await file.CopyToAsync(stream, cancellationToken);

        stream.Position = 0;
        var model = await _fileService.SaveFileAsync(fileName, stream, cancellationToken);

        await _fileService.CommitChangesAsync(cancellationToken);

        return Ok(_mapper.Map<FileInfoResponse>(model));
    }
}
