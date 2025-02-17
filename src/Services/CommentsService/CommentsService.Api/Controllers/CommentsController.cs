using CommentsService.DAL;
using CommentsService.DAL.Abstractions;
using CommentsService.DAL.Repositories;
using CommentsService.Api.Models;
using CommentsService.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommentsService.Api.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly IRepository<Comment> _commentsRepository;

        public CommentsController(MongoDBContext mongoDBContext)
        {
            _commentsRepository = new MongoRepository<Comment>(mongoDBContext.Database, "Comments");
        }

        /// <summary>
        /// Получить комментарий по id
        /// </summary>
        [HttpGet("{id:guid}", Name = "GetCommentById")]
        public async Task<ActionResult<CommentResponse>> GetCommentByIdAsync(Guid id)
        {
            var comment = await _commentsRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var response = CreateCommentResponse(comment);
            return Ok(response);
        }

        /// <summary>
        /// Получить все комментарии для указанного Issue
        /// </summary>
        [HttpGet("issue-comments/{id:guid}")]
        public async Task<ActionResult<List<CommentResponse>>> GetAllCommentsByIssueId(Guid issueId)
        {
            var comments = await _commentsRepository.GetAllAsync();
            var taskComments = comments
                .Where(c => c.IssueId == issueId)
                .ToList();
            if (taskComments.Count == 0)
            {
                return Ok(new List<CommentResponse>());
            }

            var response = taskComments
                    .Select(taskComment => CreateCommentResponse(taskComment))
                    .ToList();
            return Ok(response);
        }

        /// <summary>
        /// Создать комментарий
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CommentResponse>> CreateComment(CreateCommentRequest request)
        {
          var comment = new Comment()
          {
            Id = Guid.NewGuid(),
            IssueId = request.IssueId,
            AuthorId = request.AuthorId,
            Content = request.Content,
            CreatedAtTime = DateTime.UtcNow,
            UpdatedAtTime = DateTime.UtcNow,
          };

          await _commentsRepository.AddAsync(comment);

          return CreatedAtRoute("GetCommentById", new { id = comment.Id }, null);
        }

        /// <summary>
        /// Изменить выбранный комментарий
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> EditComment(Guid id, UpdateCommentRequest request)
        {
            var comment = await _commentsRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            comment.Content = request.Content;
            comment.UpdatedAtTime = DateTime.UtcNow;

            await _commentsRepository.UpdateAsync(id, comment);

            return NoContent();
        }

        /// <summary>
        /// Удалить комментарий
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            await _commentsRepository.DeleteAsync(id);

            return NoContent();
        }

        private CommentResponse CreateCommentResponse(Comment comment)
        {
            return new CommentResponse()
            {
                Id = comment.Id,
                IssueId = comment.IssueId,
                AuthorId = comment.AuthorId,
                Content = comment.Content,
                CreatedAtTime = comment.CreatedAtTime,
                UpdatedAtTime = comment.UpdatedAtTime
            };
        }
    }
}
