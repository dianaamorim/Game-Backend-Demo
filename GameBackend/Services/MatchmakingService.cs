using GameBackend.Models;

namespace GameBackend.Services;

public class MatchmakingService
{
    private readonly Dictionary<Guid, Match> _matches = new();
    private readonly Queue<Match> _waitingQueue = new();
    private readonly object _lock = new();

    // Returns (match, isNew) — isNew = true if match just started
    public (Match match, bool isNew) JoinQueue(Player player)
    {
        lock (_lock)
        {
            if (_waitingQueue.Count > 0)
            {
                var match = _waitingQueue.Dequeue();
                match.Player2 = player;
                match.CurrentTurnPlayerId = match.Player1.PlayerId;
                match.Status = MatchStatus.InProgress;

                Console.WriteLine($"[Matchmaking] Match created: {match.MatchId} ({match.Player1.Username} vs {player.Username})");
                return (match, true);
            }
            else
            {
                // Create a placeholder match so both players can poll the same matchId
                var match = new Match
                {
                    Player1 = player,
                    Status = MatchStatus.Waiting
                };
                _matches[match.MatchId] = match;
                _waitingQueue.Enqueue(match);
                Console.WriteLine($"[Matchmaking] {player.Username} is waiting. MatchId: {match.MatchId}");
                return (match, false);
            }
        }
    }

    public Match? GetMatch(Guid matchId)
    {
        _matches.TryGetValue(matchId, out var match);
        return match;
    }

    public (bool success, string? error) MakeMove(Guid matchId, Guid playerId, int cellIndex)
    {
        lock (_lock)
        {
            if (!_matches.TryGetValue(matchId, out var match))
                return (false, "Match not found.");

            if (match.Status != MatchStatus.InProgress)
                return (false, "Match is not in progress.");

            if (match.CurrentTurnPlayerId != playerId)
                return (false, "It's not your turn.");

            if (cellIndex < 0 || cellIndex > 8)
                return (false, "Invalid cell index.");

            if (match.Board[cellIndex] != null)
                return (false, "Cell is already taken.");

            var symbol = match.Player1.PlayerId == playerId ? "X" : "O";
            match.Board[cellIndex] = symbol;

            Console.WriteLine($"[Match {matchId}] Player {playerId} placed {symbol} at cell {cellIndex}");

            if (CheckWin(match.Board, symbol))
            {
                match.Status = MatchStatus.Finished;
                match.WinnerId = playerId;
                Console.WriteLine($"[Match {matchId}] Winner: {playerId}");
            }
            else if (match.Board.All(c => c != null))
            {
                match.Status = MatchStatus.Finished;
                Console.WriteLine($"[Match {matchId}] Draw.");
            }
            else
            {
                match.CurrentTurnPlayerId = match.Player1.PlayerId == playerId
                    ? match.Player2!.PlayerId
                    : match.Player1.PlayerId;
            }

            return (true, null);
        }
    }

    private static bool CheckWin(string?[] board, string symbol)
    {
        int[][] lines =
        [
            [0, 1, 2], [3, 4, 5], [6, 7, 8], // rows
            [0, 3, 6], [1, 4, 7], [2, 5, 8], // columns
            [0, 4, 8], [2, 4, 6]              // diagonals
        ];

        return lines.Any(line => line.All(i => board[i] == symbol));
    }

    public IEnumerable<Match> GetAllMatches() => _matches.Values;
}
