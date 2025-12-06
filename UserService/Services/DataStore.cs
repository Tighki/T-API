using T_API.Shared.Models;
using System.Collections.Concurrent;

namespace UserService.Services;

public interface IDataStore
{
    UserPreferences? GetUserPreferences(string userId);
    UserPreferences SaveUserPreferences(UserPreferences preferences);
}

public class InMemoryDataStore : IDataStore
{
    private readonly ConcurrentDictionary<string, UserPreferences> _userPreferences = new();

    public UserPreferences? GetUserPreferences(string userId) =>
        _userPreferences.TryGetValue(userId, out var prefs) ? prefs : null;

    public UserPreferences SaveUserPreferences(UserPreferences preferences)
    {
        preferences.UpdatedAt = DateTime.UtcNow;
        if (!_userPreferences.ContainsKey(preferences.UserId))
            preferences.CreatedAt = DateTime.UtcNow;
        
        _userPreferences[preferences.UserId] = preferences;
        return preferences;
    }
}

