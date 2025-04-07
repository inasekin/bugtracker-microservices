using AutoMapper;

namespace FileService.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMapperProfiles(this IServiceCollection services)
    {
        var mapperConfiguration = GetMapperProfilesFromAssemblies(); //working

        mapperConfiguration.AssertConfigurationIsValid();

        services.AddSingleton<IMapper>(new Mapper(mapperConfiguration));

        return services;
    }

    private static MapperConfiguration GetMapperProfilesFromAssemblies()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        return new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(assemblies);
        });

    }
}
