using T_API.Shared.DTOs;
using T_API.Shared.Models;

namespace PortfolioService.Services;

public interface IUserServiceClient
{
    Task<UserPreferencesResponse?> GetUserPreferencesAsync(string userId);
}

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public UserServiceClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(_configuration["Services:UserService"] ?? "https://localhost:7002");
    }

    public async Task<UserPreferencesResponse?> GetUserPreferencesAsync(string userId)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
            
            var response = await _httpClient.GetAsync("/api/v1/user/preferences");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<UserPreferencesResponse>();
        }
        catch
        {
            return null;
        }
    }
}

