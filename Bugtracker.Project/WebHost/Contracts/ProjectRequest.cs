using System;
using System.Collections.Generic;

namespace Bugtracker.WebHost.Contracts
{
    public class ProjectRequest
    {
        public string Name { get; set; }
        public string SysId { get; private set; }
        public string Description { get; private set; }

        public List<string> IssueTypes { get; set; }
        public List<Guid> UserRoles { get; set; }
        public List<string> Versions { get; set; }
        public List<IssueCategoryRequest> IssueCategories { get; set; }
    }
}