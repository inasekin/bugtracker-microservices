using AutoMapper;
using FileService.DAL.Entities;
using FileService.Domain.Models;

namespace FileService.Domain.Mappers;

public class FileMappingProfile : Profile
{
    public FileMappingProfile()
    {
        CreateMap<FileModel, FileEntity>();
        CreateMap<FileEntity, FileModel>()
            .ForMember(model => model.Path, entity => entity.Ignore());
    }
}
