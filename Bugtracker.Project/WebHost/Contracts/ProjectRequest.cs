using System;
using System.Collections.Generic;

namespace Bugtracker.WebHost.Contracts
{
    public class ProjectRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; private set; }
        public Guid ParentProjectId { get; set; }

        public IEnumerable<string> IssueTypes { get; set; }
        public IDictionary<Guid, List<Guid>> UserRoles { get; set; }
        public IEnumerable<string> Versions { get; set; }
        public IEnumerable<IssueCategoryRequest> IssueCategories { get; set; }
    }
}