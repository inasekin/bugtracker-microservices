using AutoMapper;
using Bugtracker.WebHost.Contracts;
using BugTracker.Domain;
using System.Linq;

namespace Bugtracker.WebHost.Mapping
{
    public class ProjectMappingProfile : Profile
    {
        public ProjectMappingProfile()
        {
            AllowNullCollections = true;

            CreateMap<Project, ProjectResponse>()
                        .ForMember(dest => dest.UserRoles, map => map.Ignore()) // всё сложно, будем кодом
                        .ForMember(dest => dest.Versions, opt => opt.MapFrom(src => src.Versions.Select(t => t.Name)))
                        .ForMember(dest => dest.IssueTypes, opt => opt.MapFrom(src => src.IssueTypes.Select(t => t.IssueType)));

            CreateMap<ProjectRequest, Project>()
                        .ForMember(dest => dest.Id, map => map.Ignore())
                        .ForMember(dest => dest.UserRoles, map => map.Ignore())
                        .ForMember(dest => dest.IssueCategories, map => map.Ignore())
                        .ForMember(dest => dest.Versions, map => map.Ignore())
                        .ForMember(dest => dest.IssueTypes, map => map.Ignore());

            CreateMap<ProjectIssueCategory, IssueCategoryResponse>()
                        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Name));
            
            CreateMap<IssueCategoryRequest, ProjectIssueCategory>()
                        .ForMember(dest => dest.Id, map => map.Ignore())
                        .ForMember(dest => dest.ProjectId, map => map.Ignore())
                        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName));
        }
    }
}
