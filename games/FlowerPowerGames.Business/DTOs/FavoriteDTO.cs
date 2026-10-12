using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Business.DTOs;

[ExcludeFromCodeCoverage]
public class FavoriteDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }
}
