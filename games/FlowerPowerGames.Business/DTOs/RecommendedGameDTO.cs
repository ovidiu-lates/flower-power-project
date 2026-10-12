using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Business.DTOs;

[ExcludeFromCodeCoverage]
public class RecommendedGameDto
{
    public int GameId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;
}
