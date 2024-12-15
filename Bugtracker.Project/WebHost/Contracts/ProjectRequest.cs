using System;
using System.Collections.Generic;

namespace Bugtracker.WebHost.Contracts
{
    public class ProjectRequest
    {
        public string Name { get; set; }
        public string Description { get; private set; }
        public Guid? ParentProjectId { get; set; }

        public IEnumerable<string> IssueTypes { get; set; }

        // key - userId, value - list of role ids
        public IDictionary<string, List<string>> UserRoles { get; set; }
        public IEnumerable<string> Versions { get; set; }
        public IEnumerable<IssueCategoryRequest> IssueCategories { get; set; }
    }
}