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
        [Route("{issueId}")]
        public async Task<ActionResult<Issue>> GetAsync(int issueId)
        {
            var taskItem = await _uow.BugTrackerRepository.TaskItems()
                .FirstOrDefaultAsync(taskItem => taskItem.IssueId == issueId);

            if (taskItem == null)
            {
                return NotFound();
            }

            return Ok(taskItem);
        }

        [HttpGet]
        [Route("taskitems")]
        public async Task<ActionResult<List<Issue>>> GetTaskItemsAsync()
        {
            var taskItems = _uow.BugTrackerRepository.TaskItems().ToListAsync();

            return Ok(taskItems.Result);
        }

        [HttpPost]
        public async Task<ActionResult<Issue>> CreateTaskItemAsync([FromBody] IssueCreationOptions options)
        {
            if(options == null)
            {
                return BadRequest();
            }

            var issue = Issue.Save(
                options.Topic,
                options.Description,
                options.Status,
                options.Category,
                options.StartDate,
                options.EndDate,
                options.Readiness,
                options.AffectedVersion);

            if (issue.IsFailure)
            {
                return BadRequest(issue.Error);
            }

            _uow.BugTrackerRepository.AddToContext(issue.Value);
            await _uow.SaveChangesAsync();

            return Ok(issue.Value);
        }

        [HttpPut]
        [Route("{taskItemId}")]
        public async Task<ActionResult<Issue>> UpdateTaskItemAsync(int taskItemId, [FromBody] IssueUpdateOptions options)
        {
            if (options == null)
            {
                return BadRequest();
            }

            var taskItem = await _uow.BugTrackerRepository.TaskItems()
                .FirstOrDefaultAsync(taskItem => taskItem.IssueId == taskItemId);

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
                .FirstOrDefaultAsync(taskItem => taskItem.IssueId == taskItemId);

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
