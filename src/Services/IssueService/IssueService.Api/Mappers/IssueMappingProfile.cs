using AutoMapper;
using IssueService.Api.Contracts;
using IssueService.Domain.Models;

namespace IssueService.Api.Mappers
{
    public class IssueMappingProfile : Profile
    {
        public IssueMappingProfile()
        {
            CreateMap<Domain.Models.FileInfo, FileInfoDto>();

            CreateMap<FileInfoDto, Domain.Models.FileInfo>();
                //.ForMember(dest => dest.Issue, map => map.Ignore());

            CreateMap<Issue, IssueResponse>();

            CreateMap<IssueRequest, Issue>()
                        .ForMember(dest => dest.Id, map => map.Ignore())
                        .ForMember(dest => dest.Files, map => map.Ignore());
        }
    }
}
