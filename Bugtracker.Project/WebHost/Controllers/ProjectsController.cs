using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public ProjectsController(IProjectRepository projects, IUnitOfWork unitOfWork)
        {
            _projects = projects;
            _unitOfWork = unitOfWork;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsAsync()
        {
            var customers =  await _projects.GetAllAsync();

            var response = new ProjectResponse();
            //var response = customers.Select(x => new CustomerShortResponse()
            //{
            //    Id = x.Id,
            //    Email = x.Email,
            //    FirstName = x.FirstName,
            //    LastName = x.LastName
            //}).ToList();

            // TODO:

            return Ok(response);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProjectResponse>> GetProjectAsync(Guid id)
        {
            var customer =  await _projects.GetAsync(id);
            var response = new ProjectResponse();

            // TODO:

            return Ok(response);
        }
        
        [HttpPost]
        public async Task<ActionResult<ProjectResponse>> CreateProjectAsync(ProjectRequest request)
        {
            //Получаем предпочтения из бд и сохраняем большой объект
            // TODO:
            Project project = new();
            _projects.Add(project);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjectAsync), new {id = project.Id}, null);
        }
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProjectAsync(Guid id, ProjectRequest request)
        {
            var customer = await _projects.GetAsync(id);
            
            if (customer == null)
                return NotFound();

            //TODO:
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
    }
}