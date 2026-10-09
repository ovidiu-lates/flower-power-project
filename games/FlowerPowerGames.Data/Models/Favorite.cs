using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Data.Models;

[ExcludeFromCodeCoverage]
public class Favorite
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;

    public User User { get; set; } = null!;
}
