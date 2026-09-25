using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FlowerPowerGames.Data.Models;

public class Rating
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GameId { get; set; }
    public int Score { get; set; }
    public string? Review { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Game Game { get; set; } = null!;
}
