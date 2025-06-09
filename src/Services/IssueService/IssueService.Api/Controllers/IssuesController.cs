using AutoMapper;
using Common.Validation;
using FluentValidation;
using IssueService.Api.Contracts;
using IssueService.DAL;
using IssueService.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using FileInfo = IssueService.Domain.Models.FileInfo;

namespace Bugtracker.WebHost.Controllers
{
    /// <summary>
    /// Контроллер сущности Проекты
    /// </summary>
    [ApiController]
    [Route("api/issues")]
    public class IssuesController
        : ControllerBase
    {
        private readonly IssueRepository _issues;
        private readonly IMapper _mapper;
        private readonly ICommonValidator<IssueRequest> _validator;

        public IssuesController(
            IssueRepository issues,
            IMapper mapper,
            ICommonValidator<IssueRequest> validator)
        {
            _issues = issues;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IssueResponse>>> GetIssuesAsync(string? projectId = null)
        {
            IEnumerable<Issue> projects;
            if (projectId != null)
            {
                Guid projectGuid = Guid.Parse(projectId);
                projects = await _issues.GetAllAsync(q =>
                {
                    return q.Where(i => i.ProjectId == projectGuid);
                });
            }
            else
            {
                projects = await _issues.GetAllAsync();
            }

            IEnumerable<IssueResponse> projectsResponse = projects.Select(p => MapProject(p));
            return Ok(projectsResponse);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<IssueResponse>> GetIssueAsync(Guid id)
        {
            var issue = await _issues.GetAsync(id);
            if (issue == null)
                return NotFound();

            var files = issue.Files;

            IssueResponse response = MapProject(issue);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<IssueResponse>> CreateIssuesAsync(IssueRequest request)
        {
            _validator.ValidateAndThrow(request);

            Issue issue = new();
            MapProject(request, issue);
            
            // Устанавливаем даты создания и обновления
            issue.CreatedAt = DateTime.UtcNow;
            issue.UpdatedAt = DateTime.UtcNow;
            
            // Если AuthorId не указан в запросе, можно попробовать получить из контекста
            if (issue.AuthorId == Guid.Empty)
            {
                // TODO: Получить ID пользователя из JWT токена
                // Пока что используем значение из запроса или генерируем новый GUID
                issue.AuthorId = request.AuthorId != Guid.Empty ? request.AuthorId : Guid.NewGuid();
            }
            
            await _issues.AddAsync(issue);

            var issueResponse = MapProject(issue);
            return Ok(issueResponse);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateIssuesAsync(Guid id, IssueRequest request)
        {
            var issue = await _issues.GetAsync(id);
            if (issue == null)
                return NotFound();

            MapProject(request, issue);
            
            // Обновляем дату изменения
            issue.UpdatedAt = DateTime.UtcNow;

            await _issues.UpdateAsync(issue);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteIssuesAsync(Guid id)
        {
            var project = await _issues.GetAsync(id);

            if (project == null)
                return NotFound();

            await _issues.DeleteAsync(id);
            return NoContent();
        }

        #region Helpers

        private IssueResponse MapProject(Issue issue)
        {
            IssueResponse response = _mapper.Map<Issue, IssueResponse>(issue);
            return response;
        }

        /*private FileInfo MapProject(FileInfoDto fi)
        {
            FileInfo response = _mapper.Map<FileInfoDto, FileInfo>(fi);
            return response;
        }*/

        private void MapProject(IssueRequest request, Issue issue)
        {
            _mapper.Map(request, issue);

            var updatedFiles = new List<FileInfo>();
            if (request.Files != null)
            {
                foreach (var r in request.Files)
                {
                    var i = issue.Files?.FirstOrDefault(i => i.Id == r.Id);
                    if (i != null)
                        updatedFiles.Add(i);
                    else
                        updatedFiles.Add(_mapper.Map<FileInfoDto, FileInfo>(r));
                }
            }
            issue.Files = updatedFiles;
        }

        #endregion
    }
}
