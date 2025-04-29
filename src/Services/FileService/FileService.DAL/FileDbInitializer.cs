using Microsoft.Extensions.DependencyInjection;

namespace FileService.DAL;
public static class FileDbInitializer
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FileDbContext>();
        dbContext.Database.EnsureCreated();
    }
}
