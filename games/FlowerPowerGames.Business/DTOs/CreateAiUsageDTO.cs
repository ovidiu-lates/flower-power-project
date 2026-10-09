using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Business.DTOs;

[ExcludeFromCodeCoverage]
public class CreateAiUsageDto
{
    public int TotalAvailablePrompt { get; set; }
}
