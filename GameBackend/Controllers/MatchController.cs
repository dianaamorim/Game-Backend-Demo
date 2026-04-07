using GameBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class MatchController : ControllerBase
{
    private readonly MatchmakingService _matchmaking;

    public MatchController(MatchmakingService matchmaking)
    {
        _matchmaking = matchmaking;
    }

    [HttpGet("{matchId}")]
    public IActionResult GetMatch(Guid matchId)
    {
        var match = _matchmaking.GetMatch(matchId);
        if (match == null) return NotFound("Match not found.");

        return Ok(new
        {
            matchId = match.MatchId,
            status = match.Status.ToString(),
            board = match.Board,
            currentTurnPlayerId = match.CurrentTurnPlayerId,
            player1 = new { match.Player1.PlayerId, match.Player1.Username },
            player2 = match.Player2 == null ? null : new { match.Player2.PlayerId, match.Player2.Username },
            winnerId = match.WinnerId
        });
    }

    [HttpPost("{matchId}/move")]
    public IActionResult MakeMove(Guid matchId, [FromBody] MoveRequest request)
    {
        var (success, error) = _matchmaking.MakeMove(matchId, request.PlayerId, request.CellIndex);
        if (!success) return BadRequest(error);

        var match = _matchmaking.GetMatch(matchId)!;
        return Ok(new
        {
            matchId = match.MatchId,
            status = match.Status.ToString(),
            board = match.Board,
            currentTurnPlayerId = match.CurrentTurnPlayerId,
            winnerId = match.WinnerId
        });
    }

    [HttpGet]
    public IActionResult GetAllMatches()
    {
        var matches = _matchmaking.GetAllMatches().Select(m => new
        {
            matchId = m.MatchId,
            status = m.Status.ToString(),
            player1 = m.Player1.Username,
            player2 = m.Player2?.Username ?? "waiting...",
            winnerId = m.WinnerId,
            createdAt = m.CreatedAt
        });

        return Ok(matches);
    }
}

public class MoveRequest
{
    public Guid PlayerId { get; set; }
    public int CellIndex { get; set; }
}
