using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Business.AI;

[ExcludeFromCodeCoverage]
public sealed class AiRecommendationResult
{
    public List<AiRecommendedGame> Recommendations { get; set; } = [];

    public AiTokenUsage Usage { get; set; } = new();
}

public sealed class AiRecommendedGame
{
    public int GameId { get; set; }

    public string Reason { get; set; } = string.Empty;
}

public sealed class AiTokenUsage
{
    public int InputTokens { get; set; }

    public int OutputTokens { get; set; }

    public int TotalTokens { get; set; }
}