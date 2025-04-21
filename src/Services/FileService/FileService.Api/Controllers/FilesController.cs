using AutoMapper;
using FileService.Api.Contracts;
using FileService.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController(IFileService fileService, IMapper mapper) : ControllerBase
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
    [Consumes("multipart/form-data")]
    [DisableRequestSizeLimit]
    public async Task<ActionResult<FileInfoResponse[]>> AddFiles(IFormFileCollection files, CancellationToken cancellationToken)
    {
        var request = HttpContext.Request;
        IFormFileCollection formFiles = request.Form.Files;
        var filesInfo = new FileInfoResponse[formFiles.Count];
        for(int i = 0; i < formFiles.Count; i++) {
            var fi = await UploadFile(formFiles[i], null, cancellationToken);
            filesInfo[i] = fi;
        }
        return Ok(filesInfo);
    }

    [HttpPost]
    private async Task<ActionResult<FileInfoResponse>> AddFile(IFormFile file, [FromQuery] string? fileName, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        var res = await UploadFile(file, fileName, cancellationToken);
        return Ok(res);
    }

    private async Task<FileInfoResponse> UploadFile(IFormFile file, string? fileName, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(file.FileName);
        fileName = string.IsNullOrWhiteSpace(fileName) ? file.FileName : $"{fileName}{extension}";

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);

        stream.Position = 0;
        var model = await _fileService.SaveFileAsync(fileName, stream, cancellationToken);

        //await _fileService.CommitChangesAsync(cancellationToken);

        return _mapper.Map<FileInfoResponse>(model);
    }

}
