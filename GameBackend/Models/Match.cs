namespace GameBackend.Models;

public enum MatchStatus { Waiting, InProgress, Finished }

public class Match
{
    public Guid MatchId { get; set; } = Guid.NewGuid();
    public Player Player1 { get; set; } = null!;
    public Player? Player2 { get; set; }
    public string?[] Board { get; set; } = new string?[9]; // index 0-8, "X" or "O"
    public Guid CurrentTurnPlayerId { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Waiting;
    public Guid? WinnerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
