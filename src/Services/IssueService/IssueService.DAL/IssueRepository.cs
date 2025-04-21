using IssueService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueService.DAL;

public class IssueRepository
{ 
  private readonly ApplicationDbContext _context;

  public IssueRepository(ApplicationDbContext dbContext)
  {
      _context = dbContext;
  }

  public async Task<Issue?> GetAsync(Guid id)
  {
    return await _context.Issues
            .Include(i=>i.Files)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Id == id);
  }

    public async Task<List<Issue>> GetAllAsync()
    {
        return await _context.Issues.ToListAsync();
    }

    public async Task<List<Issue>> GetAllAsync(Func<IQueryable<Issue>, IQueryable<Issue>> filter)
    {
        var query = filter(_context.Issues);
        return await query.ToListAsync();
    }

    public async Task<Issue> AddAsync(Issue issue)
    {
        _context.Issues.Add(issue);
        foreach (var file in issue.Files)
            _context.Files.Add(file);
        await _context.SaveChangesAsync();
        return issue;
    }

    public async Task UpdateAsync(Issue issue)
    {
        List<Guid> oldIds = issue.Files.Select(f=>f.Id).ToList();
        var dbFiles = _context.Files.Where(f => oldIds.Contains(f.Id)).Select(f=>f.Id).ToList();

        foreach (var f in issue.Files)
        {
            // Так не работает, потому что мы сами ключ Id файла присваиваем и e.State в этом случае = EntityState.Modified и ошибка sql
            // var e = _context.Entry(f);
            // if(e.State == EntityState.Detached)
            bool isTracked = dbFiles.Contains(f.Id);
            if (!isTracked)
                _context.Add(f);
        }
        _context.Issues.Update(issue);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var issue = await _context.Issues.FindAsync(id);
        if (issue != null)
        {
            _context.Issues.Remove(issue);
            await _context.SaveChangesAsync();
        }
    }
}
