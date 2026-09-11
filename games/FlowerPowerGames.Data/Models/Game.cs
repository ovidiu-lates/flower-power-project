using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Data.Models;

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int PlayTimeMinutes { get; set; }
    public int LearningTimeMinutes { get; set; }
    public int MinimumAge {  get; set; }
    public string? ImageUrl { get; set; }
    public decimal Rating {  get; set; }

    public ICollection<Genre> Genres { get; set; } = [];
    public ICollection<GameType> Types { get; set; } = [];
}
