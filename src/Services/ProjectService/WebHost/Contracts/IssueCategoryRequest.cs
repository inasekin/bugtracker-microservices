using System;

namespace Bugtracker.WebHost.Contracts
{
    public class IssueCategoryRequest
    {
        public string CategoryName { get; set; }

#nullable enable
        public string? UserId { get; set; }
    }
}