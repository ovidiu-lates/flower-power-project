using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.DTOs;

public class RecommendedGameDTO
{
    public int GameId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;
}
