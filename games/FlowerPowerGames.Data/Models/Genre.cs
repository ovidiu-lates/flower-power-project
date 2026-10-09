using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Data.Models;

[ExcludeFromCodeCoverage]
public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Game> Games { get; set; } = [];

    public ICollection<UserPreference> UserPreferences { get; set; } = [];
}
