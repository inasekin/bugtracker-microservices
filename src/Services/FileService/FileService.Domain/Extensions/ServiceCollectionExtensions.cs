using Microsoft.Extensions.DependencyInjection;
using FileService.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace FileService.Domain.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfigurationSection configSection)
    {
        services.AddScoped(typeof(IFileService), typeof(LocalFileService));

        services.Configure<FileStorageSettings>(configSection);

        return services;
    }
}
