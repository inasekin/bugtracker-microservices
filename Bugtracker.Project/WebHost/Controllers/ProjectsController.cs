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
        private readonly IRepository<Project> _projects;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProjectsController(
            IRepository<Project> projects,
            IUnitOfWork unitOfWork, 
            IMapper mapper)
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
            Project project = _projects.Add(new());
            MapProject(request, project);

            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjectAsync), new {id = project.Id}, null);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProjectAsync(Guid id, ProjectRequest request)
        {
            var project = await _projects.GetAsync(id);
            if (project== null)
                return NotFound();
            
            MapProject(request, project);

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProjectAsync(Guid id)
        {
            var project = await _projects.GetAsync(id);
            
            if (project == null)
                return NotFound();

            _projects.Remove(project);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        #region Helpers
        private ProjectResponse MapProject(Project project)
        {
            ProjectResponse response = _mapper.Map<Project, ProjectResponse>(project);

            if (project.UserRoles != null)
            {
                response.UserRoles = new();
                foreach (var ur in project.UserRoles)
                {
                    if (response.UserRoles.TryGetValue(ur.UserId, out List<string> roles) == false)
                    {
                        roles = new List<string>();
                        response.UserRoles.Add(ur.UserId, roles);
                    }
                    roles.Add(ur.RoleId);
                }
            }
            return response;
        }
        
        private void MapProject(ProjectRequest request, Project project)
        {
            _mapper.Map(request, project);

            project.Versions = MapCollection(
                request.Versions, // from
                project.Versions, // to
                (dto, model) => dto == model.Name, // compare
                (dto, model) => model.Name = dto); // update

            project.IssueTypes = MapCollection(
                request.IssueTypes, 
                project.IssueTypes,
                (dto, model) => dto == model.IssueType,
                (dto, model) => model.IssueType = dto);

            project.IssueCategories = MapCollection(
                request.IssueCategories, 
                project.IssueCategories,
                (dto, model) => dto.CategoryName == model.Name,
                (dto, model) => _mapper.Map(dto, model));

            // Для пользователей и ролей сложный mapping Dict<string,List> => List
            if (request.UserRoles != null)
            {
                project.UserRoles = new();
                foreach (var ur in request.UserRoles)
                {
                    string userId = ur.Key;
                    List<string> roles = ur.Value;
                    foreach(string role in roles)
                    {
                        var u = project.UserRoles.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == role);
                        if (u != null)
                            project.UserRoles.Add(u);
                        else
                        {
                            project.UserRoles.Add(new()
                            {
                                UserId = userId,
                                RoleId = role
                            });
                        }
                    }
                }
            }
        }

        private List<TModel> MapCollection<TDto, TModel>(IEnumerable<TDto> dtoCollection, IEnumerable<TModel> modelCollection, Func<TDto, TModel, bool> predicate, Action<TDto, TModel> map) where TModel : BaseEntity, new()
        {
            if(dtoCollection == null)
                return modelCollection?.ToList();
            IEnumerable<TModel> oldItems = modelCollection;
            List<TModel> newItems = new();
            foreach (TDto item in dtoCollection)
            {
                TModel oldItem = oldItems?.FirstOrDefault(i => predicate(item, i));
                TModel newItem = oldItem ?? new();
                map(item, newItem);
                newItems.Add(newItem);
            }
            return newItems;
        }

        #endregion
    }
}