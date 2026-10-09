using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Business.DTOs;
    
[ExcludeFromCodeCoverage]

public class RatingDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }

    public int Score { get; set; }

    public string? Review { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
