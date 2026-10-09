using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Data.Models;

[ExcludeFromCodeCoverage]
public class GameType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Game> Games { get; set; } = [];

    public ICollection<UserPreference> UserPreferences { get; set; } = [];
}
