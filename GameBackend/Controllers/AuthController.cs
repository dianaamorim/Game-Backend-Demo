using Microsoft.AspNetCore.Mvc;

namespace GameBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Temporary fake login
        return Ok(new { playerId = Guid.NewGuid(), username = request.Username });
    }
}

public class LoginRequest
{
    public string Username { get; set; }
}