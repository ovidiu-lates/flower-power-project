using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Business.DTOs;

[ExcludeFromCodeCoverage]
public class AiUsageDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TotalRequests { get; set; }

    public int TotalPromptUsed { get; set; }

    public int TotalAvailablePrompt { get; set; }

}
