using System;

namespace Bugtracker.WebHost.Contracts
{
    public class IssueCategoryResponse
    {
        public string CategoryName { get; set; }

        public Guid UserId { get; set; }
    }
}