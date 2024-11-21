using BugTracker.WebApplication.Data;
using BugTracker.WebApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.WebApplication.Services
{
    public class UserService
    {
        private readonly BugTrackerDbContext _context;

        public UserService(BugTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserResponse>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<UserResponse?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
        
        public async Task<UserResponse?> GetUserByPasswordHashAsync(string hash)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PasswordHash == hash);
        }
        
        public async Task<UserResponse?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task CreateUserAsync(UserResponse userResponse)
        {
            _context.Users.Add(userResponse);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(UserResponse userResponse)
        {
            _context.Users.Update(userResponse);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await GetUserByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}