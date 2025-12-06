using T_API.Shared.DTOs;

namespace PortfolioService.Services;

public interface ICoolingServiceClient
{
    Task<List<CoolingPeriodResponse>?> GetCoolingRangesAsync(string userId);
}

public class CoolingServiceClient : ICoolingServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public CoolingServiceClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(_configuration["Services:CoolingService"] ?? "https://localhost:7001");
    }

    public async Task<List<CoolingPeriodResponse>?> GetCoolingRangesAsync(string userId)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
            
            var response = await _httpClient.GetAsync("/api/v1/cooling/cooling-ranges");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<List<CoolingPeriodResponse>>();
        }
        catch
        {
            return null;
        }
    }
}

