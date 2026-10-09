using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Data.Models;

[ExcludeFromCodeCoverage]
public class AiUsage
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TotalRequests { get; set; }
    public int TotalPromptUsed { get; set; } = 0;
    public int TotalAvailablePrompt { get; set; } = 750;

    public User User { get; set; } = null!;
}