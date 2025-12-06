using Microsoft.AspNetCore.Mvc;
using T_API.Shared.Constants;
using HashService.Services;

namespace HashService.Controllers;

[ApiController]
[Route(Routes.Hash)]
public class HashController(IHashService hashService) : ControllerBase
{
    [HttpPost("sha256")]
    public IActionResult Sha256([FromBody] HashRequest req)
    {
        if (string.IsNullOrEmpty(req.Text))
            return BadRequest(new { error = "Text required" });
        return Ok(new { hash = hashService.HashSha256(req.Text, req.Salt) });
    }

    [HttpPost("md5")]
    public IActionResult Md5([FromBody] HashRequest req)
    {
        if (string.IsNullOrEmpty(req.Text))
            return BadRequest(new { error = "Text required" });
        return Ok(new { hash = hashService.HashMd5(req.Text, req.Salt) });
    }

    [HttpPost("verify")]
    public IActionResult Verify([FromBody] VerifyRequest req)
    {
        if (string.IsNullOrEmpty(req.Text) || string.IsNullOrEmpty(req.Hash))
            return BadRequest(new { error = "Text and Hash required" });
        return Ok(new { valid = hashService.Verify(req.Text, req.Hash, req.Salt) });
    }

    [HttpGet("salt")]
    public IActionResult Salt([FromQuery] int length = 64) =>
        Ok(new { salt = hashService.GenerateSalt(length) });
}

public record HashRequest(string? Text, byte[]? Salt);
public record VerifyRequest(string? Text, string? Hash, byte[]? Salt);
