namespace FileService.DAL.Repositories;

public class FileDbUnitOfWork(FileDbContext dbContext) : UnitOfWorkBase(dbContext), IUnitOfWork
{
}
