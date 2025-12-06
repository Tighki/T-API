using Microsoft.AspNetCore.Mvc;
using T_API.Shared.DTOs;
using T_API.Shared.Models;
using CoolingService.Services;

namespace CoolingService.Controllers;

[ApiController]
[Route("api/v1/cooling")]
public class CoolingPeriodController : ControllerBase
{
    private readonly IDataStore _dataStore;
    private readonly ILogger<CoolingPeriodController> _logger;

    public CoolingPeriodController(IDataStore dataStore, ILogger<CoolingPeriodController> logger)
    {
        _dataStore = dataStore;
        _logger = logger;
    }

    /// <summary>
    /// Получить все диапазоны охлаждения для пользователя
    /// </summary>
    [HttpGet("cooling-ranges")]
    [ProducesResponseType(typeof(List<CoolingPeriodResponse>), StatusCodes.Status200OK)]
    public IActionResult GetCoolingRanges([FromHeader(Name = "X-User-Id")] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest(new { error = "User ID is required in X-User-Id header" });

        var periods = _dataStore.GetCoolingPeriods(userId);
        var response = periods.Select(p => new CoolingPeriodResponse
        {
            Id = p.Id,
            PriceFrom = p.PriceFrom,
            PriceTo = p.PriceTo,
            CoolingDays = p.CoolingDays
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Добавить новый диапазон охлаждения
    /// </summary>
    [HttpPost("cooling-ranges")]
    [ProducesResponseType(typeof(CoolingPeriodResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult AddCoolingRange(
        [FromHeader(Name = "X-User-Id")] string userId,
        [FromBody] CoolingPeriodRequest request)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest(new { error = "User ID is required in X-User-Id header" });

        if (request.PriceFrom < 0 || request.PriceTo <= 0 || request.PriceFrom >= request.PriceTo)
            return BadRequest(new { error = "Invalid price range" });

        if (request.CoolingDays <= 0)
            return BadRequest(new { error = "Cooling days must be positive" });

        var item = new CoolingPeriodItem
        {
            UserId = userId,
            PriceFrom = request.PriceFrom,
            PriceTo = request.PriceTo,
            CoolingDays = request.CoolingDays
        };

        var saved = _dataStore.AddCoolingPeriod(item);
        var response = new CoolingPeriodResponse
        {
            Id = saved.Id,
            PriceFrom = saved.PriceFrom,
            PriceTo = saved.PriceTo,
            CoolingDays = saved.CoolingDays
        };

        return CreatedAtAction(nameof(GetCoolingRanges), new { userId }, response);
    }
}

