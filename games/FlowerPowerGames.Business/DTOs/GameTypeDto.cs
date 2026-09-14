using System.ComponentModel.DataAnnotations;

namespace FlowerPowerGames.Business.DTOs;

public class GameTypeDto
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
}
