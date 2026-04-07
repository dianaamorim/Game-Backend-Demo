namespace GameBackend.Models;

public class Player
{
    public Guid PlayerId { get; set; }
    public string Username { get; set; } = string.Empty;
}
