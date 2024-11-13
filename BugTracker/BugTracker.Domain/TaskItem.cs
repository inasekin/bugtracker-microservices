using CSharpFunctionalExtensions;
using System.Runtime.CompilerServices;

namespace BugTracker.Domain
{
    public class TaskItem
    {
        public const int MAX_TOPIC_LENGTH = 255;
        public const int MAX_DESCRIPTION_LENGTH = 1000;

        public int TaskItemId { get; private set; }
        public string Topic { get; private set; }
        public string Description { get; private set; }
        public string Status { get; private set; }
        public string Category { get; private set; }
        public DateTimeOffset StartDate { get; private set; }
        public DateTimeOffset EndDate { get; private set; }
        public int Readiness { get; private set; }
        public string AffectedVersion { get; private set; }
        //public byte[] Files { get; private set; }

        /// <summary>
        /// Техническое поле версии объекта
        /// </summary>
        public int TaskItemVersion { get; private set; }

        public static Result<TaskItem> Save(
            string topic,
            string description,
            TaskItemStatus status,
            TaskItemCategory category,
            DateTime startDate,
            DateTime endDate,
            int readiness,
            string affectedVersion)
        {
            if (string.IsNullOrEmpty(topic) || topic.Length > MAX_TOPIC_LENGTH)
            {
                return Result.Failure<TaskItem>($"{nameof(topic)} cannot be more then 255 symbols");
            }

            if (string.IsNullOrEmpty(description) || description.Length > MAX_DESCRIPTION_LENGTH)
            {
                return Result.Failure<TaskItem>($"{nameof(description)} cannot be more then 1000 symbols");
            }

            if (startDate > endDate)
            {
                return Result.Failure<TaskItem>($"{nameof(startDate)} cannot be later than the {nameof(endDate)}");
            }

            if (readiness < 1 || readiness > 100)
            {
                return Result.Failure<TaskItem>($"{nameof(readiness)} should be between 0 and 100");
            }

            var taskItem = new TaskItem()
            {
                Topic = topic,
                Description = description,
                Status = status.ToString(),
                Category = category.ToString(),
                StartDate = startDate,
                EndDate = endDate,
                Readiness = readiness,
                AffectedVersion = affectedVersion,
                TaskItemVersion = 1
            };

            return Result.Success(taskItem);
        }

        public Result<TaskItem> Update(string topic,
            string description,
            TaskItemStatus status,
            TaskItemCategory category,
            DateTime startDate,
            DateTime endDate,
            int readiness,
            string affectedVersion,
            int taskItemVersion)
        {
            if (string.IsNullOrEmpty(topic) || topic.Length > MAX_TOPIC_LENGTH)
            {
                return Result.Failure<TaskItem>($"{nameof(topic)} cannot be more then 255 symbols");
            }

            if (string.IsNullOrEmpty(description) || description.Length > MAX_DESCRIPTION_LENGTH)
            {
                return Result.Failure<TaskItem>($"{nameof(description)} cannot be more then 1000 symbols");
            }

            if (startDate > endDate)
            {
                return Result.Failure<TaskItem>($"{nameof(startDate)} cannot be later than the {nameof(endDate)}");
            }

            if (readiness < 1 || readiness > 100)
            {
                return Result.Failure<TaskItem>($"{nameof(readiness)} should be between 0 and 100");
            }

            var checkVersion = CheckVersion(taskItemVersion);
            if (checkVersion.IsFailure)
            {
                return Result.Failure<TaskItem>(checkVersion.Error);
            }

            this.Topic = topic;
            this.Description = description;
            this.Status = status.ToString();
            this.Category = category.ToString();
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Readiness = readiness;
            this.AffectedVersion = affectedVersion;
            this.TaskItemVersion += 1;

            return Result.Success(this);
        }

        private Result CheckVersion(int taskItemVersion)
        {
            if (taskItemVersion != this.TaskItemVersion)
            {
                return Result
                    .Failure($"The state of request card # {this.TaskItemId} has been changed by another user");
            }

            return Result.Success();
        }

    }
}
