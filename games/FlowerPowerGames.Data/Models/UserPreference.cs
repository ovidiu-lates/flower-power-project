using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Data.Models;

[ExcludeFromCodeCoverage]
public class UserPreference
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public decimal minBudget { get; set; }

    public decimal maxBudget { get; set; }

    public int minPlayTime { get; set; }

    public int maxPlayTime { get; set; }
    public int minPlayers { get; set; }
    public int maxPlayers { get; set; }
    public int MinimumAge { get; set; }
    public ICollection<Genre> Genres { get; set; } = [];
    public ICollection<GameType> Types { get; set; } = [];
    public User User { get; set; } = null!;
}
