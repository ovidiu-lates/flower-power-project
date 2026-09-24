using System.ComponentModel.DataAnnotations;


namespace FlowerPowerGames.Business.DTOs;

public class GameDto
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int MinPlayers { get; set; }

    public int MaxPlayers { get; set; }

    public int PlayTimeMinutes { get; set; }

    public int LearningTimeMinutes { get; set; }

    public int MinimumAge { get; set; }

    public string? ImageUrl { get; set; }

    public decimal Rating { get; set; }

    public List<int> GenreIds { get; set; } = [];

    public List<int> TypeIds { get; set; } = [];
}