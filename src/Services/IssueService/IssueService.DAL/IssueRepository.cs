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
    return await _context.Issues.FirstOrDefaultAsync(u => u.Id == id);
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

    public async Task<Issue> AddAsync(Issue user)
  {
    Issue issue = _context.Issues.Add(user).Entity;
    await _context.SaveChangesAsync();
    return issue;
  }

  public async Task UpdateAsync(Issue user)
  {
    _context.Issues.Update(user);
    await _context.SaveChangesAsync();
  }

  public async Task DeleteAsync(Guid id)
  {
    var user = await _context.Issues.FindAsync(id);
    if (user != null)
    {
      _context.Issues.Remove(user);
      await _context.SaveChangesAsync();
    }
  }
}
