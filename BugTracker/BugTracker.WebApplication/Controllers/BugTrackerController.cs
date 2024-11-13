using BugTracker.DAL;
using BugTracker.Domain;
using BugTracker.WebApplication.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.WebApplication.Controllers
{
    [ApiController]
    [Route("controller")]
    public class BugTrackerController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public BugTrackerController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        [Route("{taskItemId}")]
        public async Task<ActionResult<TaskItem>> GetAsync(int taskItemId)
        {
            var taskItem = await _uow.BugTrackerRepository.TaskItems()
                .FirstOrDefaultAsync(taskItem => taskItem.TaskItemId == taskItemId);

            if (taskItem == null)
            {
                return NotFound();
            }

            return Ok(taskItem);
        }

        [HttpGet]
        [Route("taskitems")]
        public async Task<ActionResult<List<TaskItem>>> GetTaskItemsAsync()
        {
            var taskItems = _uow.BugTrackerRepository.TaskItems().ToListAsync();

            return Ok(taskItems.Result);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTaskItemAsync([FromBody] TaskItemCreationOptions options)
        {
            if(options == null)
            {
                return BadRequest();
            }

            var taskItem = TaskItem.Save(
                options.Topic,
                options.Description,
                options.Status,
                options.Category,
                options.StartDate,
                options.EndDate,
                options.Readiness,
                options.AffectedVersion);

            if (taskItem.IsFailure)
            {
                return BadRequest(taskItem.Error);
            }

            _uow.BugTrackerRepository.AddToContext(taskItem.Value);
            await _uow.SaveChangesAsync();

            return Ok(taskItem.Value);
        }

        [HttpPut]
        [Route("{taskItemId}")]
        public async Task<ActionResult<TaskItem>> UpdateTaskItemAsync(int taskItemId, [FromBody] TaskItemUpdateOptions options)
        {
            if (options == null)
            {
                return BadRequest();
            }

            var taskItem = await _uow.BugTrackerRepository.TaskItems()
                .FirstOrDefaultAsync(taskItem => taskItem.TaskItemId == taskItemId);

            if(taskItem == null)
            {
                return NotFound();
            }

            var updatedTaskItem = taskItem.Update(
                                    options.Topic,
                                    options.Description,
                                    options.Status,
                                    options.Category,
                                    options.StartDate,
                                    options.EndDate,
                                    options.Readiness,
                                    options.AffectedVersion,
                                    options.TaskItemVersion);

            if (updatedTaskItem.IsFailure)
            {
                return BadRequest(updatedTaskItem.Error);
            }

            await _uow.SaveChangesAsync();

            return Ok(updatedTaskItem.Value);
        }


        [HttpDelete]
        [Route("{taskItemId}")]
        public async Task<ActionResult> DeleteTaskItem(int taskItemId)
        {
            var taskItem = await _uow.BugTrackerRepository.TaskItems()
                .FirstOrDefaultAsync(taskItem => taskItem.TaskItemId == taskItemId);

            if(taskItem == null)
            { 
                return NotFound(); 
            }

            _uow.BugTrackerRepository.TaskItems().Remove(taskItem);
            await _uow.SaveChangesAsync();

            return Ok();
        }

    }
}
