using Microsoft.EntityFrameworkCore;
using T_API.Shared.Models;
using UserService.Data;

namespace UserService.Services;

public interface IUserRepository
{
    Task<UserPreferences?> GetUserPreferencesAsync(string userId);
    Task<UserPreferences> SaveUserPreferencesAsync(UserPreferences preferences);
}

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<UserPreferences?> GetUserPreferencesAsync(string userId) =>
        await _context.UserPreferences.FindAsync(userId);

    public async Task<UserPreferences> SaveUserPreferencesAsync(UserPreferences preferences)
    {
        var existing = await _context.UserPreferences.FindAsync(preferences.UserId);
        
        if (existing == null)
        {
            preferences.CreatedAt = DateTime.UtcNow;
            _context.UserPreferences.Add(preferences);
        }
        else
        {
            preferences.CreatedAt = existing.CreatedAt;
            preferences.UpdatedAt = DateTime.UtcNow;
            _context.Entry(existing).CurrentValues.SetValues(preferences);
        }

        await _context.SaveChangesAsync();
        return preferences;
    }
}

