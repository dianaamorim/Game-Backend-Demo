using GameBackend.Models;
using GameBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class MatchmakingController : ControllerBase
{
    private readonly MatchmakingService _matchmaking;

    public MatchmakingController(MatchmakingService matchmaking)
    {
        _matchmaking = matchmaking;
    }

    [HttpPost("join")]
    public IActionResult Join([FromBody] JoinRequest request)
    {
        var player = new Player
        {
            PlayerId = request.PlayerId,
            Username = request.Username
        };

        var (match, isNew) = _matchmaking.JoinQueue(player);

        return Ok(new
        {
            matchId = match.MatchId,
            status = match.Status.ToString(),
            message = isNew ? "Match found! Game starting." : "Waiting for opponent..."
        });
    }
}

public class JoinRequest
{
    public Guid PlayerId { get; set; }
    public string Username { get; set; } = string.Empty;
}
