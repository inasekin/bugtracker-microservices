using System;

namespace Bugtracker.WebHost.Contracts
{
    public class IssueCategoryResponse
    {
        public string CategoryName { get; set; }

#nullable enable
        public string? UserId { get; set; }
    }
}