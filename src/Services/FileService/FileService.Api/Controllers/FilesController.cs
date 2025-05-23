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

    [HttpGet]
    public ActionResult<IEnumerable<FileInfoResponse>> GetAllFileInfo([FromQuery] int take, [FromQuery] int skip)
    {
        var fileInfo = _fileService.GetAllFileInfo(take, skip);
        if (fileInfo == null || !fileInfo.Any())
            return NoContent();

        return Ok(fileInfo.Select(_mapper.Map<FileInfoResponse>));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FileInfoResponse>> GetFileInfo([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            return BadRequest("Id is empty");

        var fileInfo = await _fileService.GetFileInfoAsync(id, cancellationToken);
        if (fileInfo == null)
            return NotFound("Информация о файле отсутствует");

        return Ok(_mapper.Map<FileInfoResponse>(fileInfo));
    }

    [HttpGet("show/{id:guid}")]
    public async Task<ActionResult> GetPhysicalFile(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            return BadRequest("Id is empty");

        var file = await _fileService.GetFileInfoAsync(id, cancellationToken);
        if (file == null)
            return NotFound("Информация о файле отсутствует");

        var fileInfo = new FileInfo(file.Path);
        if (!fileInfo.Exists)
            return NotFound($"Файл '{file.Path}' не найден.");

        return PhysicalFile(file.Path, GetMimeType(file.Name));
    }

    [HttpGet("download/{id:guid}")]
    public async Task<ActionResult> DownloadFile(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            return BadRequest("Id is empty");

        var file = await _fileService.GetFileInfoAsync(id, cancellationToken);
        if (file == null)
            return NotFound();

        // Проверяем, существует ли файл
        if (!System.IO.File.Exists(file.Path))
            return NotFound($"Файл '{file.Path}' не найден.");

        // Читаем файл в виде массива байтов
        var fileBytes = System.IO.File.ReadAllBytes(file.Path);

        // Определяем MIME-тип файла (например, "application/pdf" для PDF)
        var mimeType = GetMimeType(file.Name);

        // Возвращаем файл с указанием имени для скачивания
        return File(fileBytes, mimeType, file.Name);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [DisableRequestSizeLimit]
    public async Task<ActionResult<FileInfoResponse[]>> AddFiles([FromForm] IFormFileCollection files, CancellationToken cancellationToken)
    {
        //var request = HttpContext.Request;
        //files = request.Form.Files;
        if (files == null || !files.Any())
            return NoContent();

        var response = new List<FileInfoResponse>();
        foreach (var f in files)
            response.Add(await UploadFile(f, cancellationToken));
        return Ok(response);
    }

    [HttpPost]
    private async Task<ActionResult<FileInfoResponse>> AddFile(IFormFile file, [FromQuery] string? fileName, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        var res = await UploadFile(file, fileName, cancellationToken);
        return Ok(res);
    }

    private async Task<FileInfoResponse> UploadFile(IFormFile file, CancellationToken cancellationToken)
    {
        return await UploadFile(file, null, cancellationToken);
    }

    private async Task<FileInfoResponse> UploadFile(IFormFile file, string? fileName, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(file.FileName);
        fileName = string.IsNullOrWhiteSpace(fileName) ? file.FileName : $"{fileName}{extension}";

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);

        stream.Position = 0;
        var model = await _fileService.SaveFileAsync(fileName, stream, cancellationToken);

        await _fileService.CommitChangesAsync(cancellationToken);

        return _mapper.Map<FileInfoResponse>(model);
    }

    /// <summary>
    /// Определяет MIME-тип файла на основе его расширения.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    /// <returns>MIME-тип файла.</returns>
    private static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".txt" => "text/plain",
            _ => "application/octet-stream" // По умолчанию
        };
    }
}
