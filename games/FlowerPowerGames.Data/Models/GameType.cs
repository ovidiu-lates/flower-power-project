using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Data.Models;

public class GameType
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Game> Games { get; set; } = [];
}
