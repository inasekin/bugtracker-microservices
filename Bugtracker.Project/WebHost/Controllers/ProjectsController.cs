using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bugtracker.WebHost.Contracts;
using BugTracker.DataAccess;
using BugTracker.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Bugtracker.WebHost.Controllers
{
    /// <summary>
    /// Контроллер сущности Проекты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProjectsController
        : ControllerBase
    {
        private readonly IProjectRepository _projects;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProjectsController(IProjectRepository projects, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _projects = projects;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsAsync()
        {
            IEnumerable<Project> projects =  await _projects.GetAllAsync();
            IEnumerable<ProjectResponse> projectsResponse = projects.Select(p => MapProject(p));            
            return Ok(projectsResponse);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProjectResponse>> GetProjectAsync(Guid id)
        {
            var project =  await _projects.GetAsync(id);
            if (project == null)
                return NotFound();

            ProjectResponse response = MapProject(project);
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<ActionResult<ProjectResponse>> CreateProjectAsync(ProjectRequest request)        
        {
            if (request.Id != Guid.Empty)
            {
                var projectExist = await _projects.GetAsync(request.Id);
                if (projectExist == null)
                    return Conflict(); // Already exist
            }

            Project project = MapProject(request);
            _projects.Add(project);

            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjectAsync), new {id = project.Id}, null);
        }
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProjectAsync(Guid id, ProjectRequest request)
        {
            var project = await _projects.GetAsync(request.Id);
            if (project== null)
                return NotFound();
            
            MapProject(request, project);
            _projects.Add(project);

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProjectAsync(Guid id)
        {
            var customer = await _projects.GetAsync(id);
            
            if (customer == null)
                return NotFound();

            _projects.Remove(customer);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        #region Helpers
        private ProjectResponse MapProject(Project project)
        {
            ProjectResponse response = _mapper.Map<Project, ProjectResponse>(project);
            response.UserRoles = new();
            foreach (var ur in project.UserRoles)
            {
                List<Guid> roles = null;
                if (response.UserRoles.TryGetValue(ur.UserId, out roles) == false)
                {
                    roles = new List<Guid>();
                    response.UserRoles.Add(ur.UserId, roles);
                }
                roles.Add(ur.RoleId);
            }
            return response;
        }

        private Project MapProject(ProjectRequest request)
        {
            Project project = _mapper.Map<ProjectRequest, Project>(request);
            if (project.Id == Guid.Empty)
                project.Id = Guid.NewGuid();
            project.Versions = new List<ProjectVersion>();
            foreach (string versionId in request.Versions)
                project.Versions.Add(new ProjectVersion() { Id = Guid.NewGuid(), Name = versionId, ProjectId = project.Id });
            project.IssueTypes = new List<ProjectIssueType>();
            foreach (string name in request.IssueTypes)
                project.IssueTypes.Add(new ProjectIssueType() { Id = Guid.NewGuid(), IssueType = name, ProjectId = project.Id });
            foreach (IssueCategoryRequest cat in request.IssueCategories)
                project.IssueCategories.Add(new ProjectIssueCategory() { Id = Guid.NewGuid(), Name = cat.CategoryName, UserId = cat.UserId, ProjectId = project.Id });
            return project;
        }

        private void MapProject(ProjectRequest request, Project project)
        {
            List<ProjectVersion> oldVersions = project.Versions;
            project.Versions = new List<ProjectVersion>();
            foreach (string name in request.Versions)
            {
                project.Versions.Add(new ProjectVersion()
                {
                    Id = GetOrCreateGuid<ProjectVersion>(oldVersions, i => i.Name == name, i => i.Id),
                    Name = name,
                    ProjectId = project.Id
                });
            }

            List<ProjectIssueType> oldIssueTypes = project.IssueTypes;
            project.IssueTypes = new List<ProjectIssueType>();
            foreach (string name in request.IssueTypes)
            {
                project.IssueTypes.Add(new ProjectIssueType()
                {
                    Id = GetOrCreateGuid(oldIssueTypes, i => i.IssueType == name, i => i.Id),
                    IssueType = name,
                    ProjectId = project.Id
                });
            }

            List<ProjectIssueCategory> oldIssueCategories = project.IssueCategories;
            project.IssueCategories = new List<ProjectIssueCategory>();
            foreach (var cat in request.IssueCategories)
            {
                project.IssueCategories.Add(new ProjectIssueCategory()
                {
                    Id = GetOrCreateGuid(oldIssueCategories, i => i.Name == cat.CategoryName, i => i.Id),
                    Name = cat.CategoryName,
                    UserId = cat.UserId,
                    ProjectId = project.Id
                });
            }
        }

        private Guid GetOrCreateGuid<T>(IEnumerable<T> col, Func<T, bool> predicate, Func<T, Guid> selector)
        {
            T o = col.FirstOrDefault(predicate);
            if (o == null)
                return Guid.NewGuid();
            else
                return selector(o);
        }

        #endregion
    }
}