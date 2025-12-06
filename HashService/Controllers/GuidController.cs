using Microsoft.AspNetCore.Mvc;
using T_API.Shared.Constants;
using HashService.Services;

namespace HashService.Controllers;

[ApiController]
[Route(Routes.Guid)]
public class GuidController(IGuidService guidService) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { guid = guidService.Generate() });

    [HttpGet("batch")]
    public IActionResult Batch([FromQuery] int count = 5) =>
        Ok(new { guids = Enumerable.Range(0, Math.Min(count, 100)).Select(_ => guidService.Generate()) });
}
