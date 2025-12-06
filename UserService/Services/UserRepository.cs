using T_API.Shared.Models;
using UserService.Data;

namespace UserService.Services;

public interface IUserRepository
{
    Task<UserPreferences?> GetUserPreferencesAsync(int userId);
    Task<UserPreferences> SaveUserPreferencesAsync(UserPreferences preferences);
}

public class UserRepository(UserDbContext context) : IUserRepository
{
    public async Task<UserPreferences?> GetUserPreferencesAsync(int userId) =>
        await context.UserPreferences.FindAsync(userId);

    public async Task<UserPreferences> SaveUserPreferencesAsync(UserPreferences prefs)
    {
        var existing = await context.UserPreferences.FindAsync(prefs.UserId);
        if (existing == null)
        {
            prefs.CreatedAt = DateTime.UtcNow;
            context.UserPreferences.Add(prefs);
        }
        else
        {
            prefs.CreatedAt = existing.CreatedAt;
            prefs.UpdatedAt = DateTime.UtcNow;
            context.Entry(existing).CurrentValues.SetValues(prefs);
        }
        await context.SaveChangesAsync();
        return prefs;
    }
}
