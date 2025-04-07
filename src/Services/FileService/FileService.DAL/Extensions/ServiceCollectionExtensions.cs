using FileService.DAL.Repositories;
using FileService.DAL.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.DAL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, string connectionString)
    {
        services
            .AddDbContext<FileDbContext>(options =>
                options.UseNpgsql(connectionString)
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine))

            .AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>))

            .AddScoped<IUnitOfWork, FileDbUnitOfWork>();

        return services;
    }
}
