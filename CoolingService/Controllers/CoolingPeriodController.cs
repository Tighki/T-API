using Microsoft.AspNetCore.Mvc;
using T_API.Shared.DTOs;
using T_API.Shared.Models;
using T_API.Shared.Validation;
using T_API.Shared.Constants;
using CoolingService.Services;

namespace CoolingService.Controllers;

[ApiController]
[Route(Routes.Cooling)]
public class CoolingPeriodController(ICoolingRepository repository) : ControllerBase
{
    [HttpGet("cooling-ranges")]
    public async Task<IActionResult> GetCoolingRanges([FromHeader(Name = Headers.UserId)] int userId)
    {
        if (!ValidationRules.IsValidUserId(userId))
            return BadRequest(new { error = ErrorMessages.InvalidUserId });
        var periods = await repository.GetCoolingPeriodsAsync(userId);
        return Ok(periods.Select(p => new CoolingPeriodResponse
        {
            Id = p.Id, PriceFrom = p.PriceFrom, PriceTo = p.PriceTo, CoolingDays = p.CoolingDays
        }));
    }

    [HttpPost("cooling-ranges")]
    public async Task<IActionResult> AddCoolingRange([FromHeader(Name = Headers.UserId)] int userId, [FromBody] CoolingPeriodRequest req)
    {
        if (!ValidationRules.IsValidUserId(userId))
            return BadRequest(new { error = ErrorMessages.InvalidUserId });
        if (req.PriceFrom < 0 || !ValidationRules.IsValidPrice(req.PriceTo) || req.PriceFrom >= req.PriceTo)
            return BadRequest(new { error = ErrorMessages.InvalidPriceRange });
        if (req.CoolingDays <= 0 || req.CoolingDays > 365)
            return BadRequest(new { error = ErrorMessages.CoolingDaysRange });

        var item = new CoolingPeriodItem { UserId = userId, PriceFrom = req.PriceFrom, PriceTo = req.PriceTo, CoolingDays = req.CoolingDays };
        var saved = await repository.AddCoolingPeriodAsync(item);
        return CreatedAtAction(nameof(GetCoolingRanges), new { userId },
            new CoolingPeriodResponse { Id = saved.Id, PriceFrom = saved.PriceFrom, PriceTo = saved.PriceTo, CoolingDays = saved.CoolingDays });
    }
}
