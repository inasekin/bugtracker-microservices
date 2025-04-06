namespace FileService.DAL.Repositories;
public interface IUnitOfWork
{
    public Task SaveChanges(CancellationToken cancellationToken);
}
