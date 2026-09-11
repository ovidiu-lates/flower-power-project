using System.ComponentModel.DataAnnotations;


namespace FlowerPowerGames.Business.DTOs;

public class GameDto
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue)]
    public int MinPlayers { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxPlayers { get; set; }

    [Range(1, int.MaxValue)]
    public int PlayTimeMinutes { get; set; }

    [Range(0, int.MaxValue)]
    public int LearningTimeMinutes { get; set; }

    [Range(0, int.MaxValue)]
    public int MinimumAge { get; set; }

    public string? ImageUrl { get; set; }

    public decimal Rating { get; set; }

    public List<GenreDto> Genres { get; set; } = [];

    public List<GameTypeDto> Types { get; set; } = [];
}