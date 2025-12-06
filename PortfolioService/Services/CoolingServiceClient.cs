using T_API.Shared.DTOs;
using T_API.Shared.Constants;

namespace PortfolioService.Services;

public interface ICoolingServiceClient
{
    Task<List<CoolingPeriodResponse>?> GetCoolingRangesAsync(int userId);
}

public class CoolingServiceClient(HttpClient http, IConfiguration config) : ICoolingServiceClient
{
    public async Task<List<CoolingPeriodResponse>?> GetCoolingRangesAsync(int userId)
    {
        try
        {
            http.BaseAddress ??= new Uri(config["Services:CoolingService"] ?? "http://localhost:5001");
            http.DefaultRequestHeaders.Clear();
            http.DefaultRequestHeaders.Add(Headers.UserId, userId.ToString());
            var resp = await http.GetAsync("/api/v1/cooling/cooling-ranges");
            return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<List<CoolingPeriodResponse>>() : null;
        }
        catch { return null; }
    }
}
