using System;

namespace Bugtracker.WebHost.Contracts
{
    public class IssueCategoryRequest
    {
        public string CategoryName { get; set; }
        public Guid UserId { get; set; }
    }
}