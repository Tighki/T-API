using T_API.Shared.DTOs;
using T_API.Shared.Constants;

namespace PortfolioService.Services;

public interface IUserServiceClient
{
    Task<UserPreferencesResponse?> GetUserPreferencesAsync(int userId);
}

public class UserServiceClient(HttpClient http, IConfiguration config) : IUserServiceClient
{
    public async Task<UserPreferencesResponse?> GetUserPreferencesAsync(int userId)
    {
        try
        {
            http.BaseAddress ??= new Uri(config["Services:UserService"] ?? "http://localhost:5002");
            http.DefaultRequestHeaders.Clear();
            http.DefaultRequestHeaders.Add(Headers.UserId, userId.ToString());
            var resp = await http.GetAsync("/api/v1/user/preferences");
            return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<UserPreferencesResponse>() : null;
        }
        catch { return null; }
    }
}
