using AutoMapper;
using FileService.Api.Contracts;
using FileService.Domain.Models;

namespace FileService.Api.Mappers;

public sealed class FileMappingProfile : Profile
{
    public FileMappingProfile()
    {
        CreateMap<FileModel, FileInfoResponse>();
        CreateMap<FileInfoResponse, FileModel>();
    }
}
