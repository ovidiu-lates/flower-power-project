using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Data.Models;

public class Favorite
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
}
