using AutoMapper;
using IssueService.Api.Contracts;
using IssueService.Domain.Models;

namespace IssueService.Api.Mappers
{
    public class IssueMappingProfile : Profile
    {
        public IssueMappingProfile()
        {
            CreateMap<Issue, IssueResponse>();

            CreateMap<IssueRequest, Issue>()
                        .ForMember(dest => dest.Id, map => map.Ignore());
        }
    }
}
