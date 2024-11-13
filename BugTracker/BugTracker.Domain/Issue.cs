using CSharpFunctionalExtensions;
using System.Runtime.CompilerServices;

namespace BugTracker.Domain
{
    public class Issue
    {
        public const int MAX_TOPIC_LENGTH = 255;
        public const int MAX_DESCRIPTION_LENGTH = 1000;

        public int IssueId { get; private set; }
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
        public int IssueVersion { get; private set; }

        public static Result<Issue> Save(
            string topic,
            string description,
            IssueStatus status,
            IssueCategory category,
            DateTime startDate,
            DateTime endDate,
            int readiness,
            string affectedVersion)
        {
            if (string.IsNullOrEmpty(topic) || topic.Length > MAX_TOPIC_LENGTH)
            {
                return Result.Failure<Issue>($"{nameof(topic)} cannot be more then 255 symbols");
            }

            if (string.IsNullOrEmpty(description) || description.Length > MAX_DESCRIPTION_LENGTH)
            {
                return Result.Failure<Issue>($"{nameof(description)} cannot be more then 1000 symbols");
            }

            if (startDate > endDate)
            {
                return Result.Failure<Issue>($"{nameof(startDate)} cannot be later than the {nameof(endDate)}");
            }

            if (readiness < 1 || readiness > 100)
            {
                return Result.Failure<Issue>($"{nameof(readiness)} should be between 0 and 100");
            }

            var taskItem = new Issue()
            {
                Topic = topic,
                Description = description,
                Status = status.ToString(),
                Category = category.ToString(),
                StartDate = startDate,
                EndDate = endDate,
                Readiness = readiness,
                AffectedVersion = affectedVersion,
                IssueVersion = 1
            };

            return Result.Success(taskItem);
        }

        public Result<Issue> Update(string topic,
            string description,
            IssueStatus status,
            IssueCategory category,
            DateTime startDate,
            DateTime endDate,
            int readiness,
            string affectedVersion,
            int taskItemVersion)
        {
            if (string.IsNullOrEmpty(topic) || topic.Length > MAX_TOPIC_LENGTH)
            {
                return Result.Failure<Issue>($"{nameof(topic)} cannot be more then 255 symbols");
            }

            if (string.IsNullOrEmpty(description) || description.Length > MAX_DESCRIPTION_LENGTH)
            {
                return Result.Failure<Issue>($"{nameof(description)} cannot be more then 1000 symbols");
            }

            if (startDate > endDate)
            {
                return Result.Failure<Issue>($"{nameof(startDate)} cannot be later than the {nameof(endDate)}");
            }

            if (readiness < 1 || readiness > 100)
            {
                return Result.Failure<Issue>($"{nameof(readiness)} should be between 0 and 100");
            }

            var checkVersion = CheckVersion(taskItemVersion);
            if (checkVersion.IsFailure)
            {
                return Result.Failure<Issue>(checkVersion.Error);
            }

            this.Topic = topic;
            this.Description = description;
            this.Status = status.ToString();
            this.Category = category.ToString();
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Readiness = readiness;
            this.AffectedVersion = affectedVersion;
            this.IssueVersion += 1;

            return Result.Success(this);
        }

        private Result CheckVersion(int taskItemVersion)
        {
            if (taskItemVersion != this.IssueVersion)
            {
                return Result
                    .Failure($"The state of request card # {this.IssueId} has been changed by another user");
            }

            return Result.Success();
        }

    }
}
