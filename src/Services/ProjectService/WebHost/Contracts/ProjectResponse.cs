using System;
using System.Collections.Generic;

namespace Bugtracker.WebHost.Contracts
{
    public class ProjectResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ParentProjectId { get; set; }
        public List<string> IssueTypes { get; set; }

        // key - userId, value - list of role ids
        public Dictionary<string, List<string>> UserRoles { get; set; }
        public List<string> Versions { get; set; }
        public List<IssueCategoryResponse> IssueCategories { get; set; }

        public ProjectResponse()
        {
            
        }
    }
}