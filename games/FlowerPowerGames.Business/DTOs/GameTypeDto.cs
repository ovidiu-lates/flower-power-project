using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Business.DTOs;

[ExcludeFromCodeCoverage]
public class GameTypeDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
