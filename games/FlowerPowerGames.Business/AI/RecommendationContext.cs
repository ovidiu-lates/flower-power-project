using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Business.AI;

[ExcludeFromCodeCoverage]
public sealed class RecommendationContext
{
    public string UserMessage { get; set;  } = string.Empty;
    
    public AiUserPreferenceContext? Preference { get; set; }

    public List<AiGameContext> FavoriteGames { get; set; } = [];

    public List<AiGameContext> CandidateGames { get; set; } = [];
}
[ExcludeFromCodeCoverage]
public sealed class AiUserPreferenceContext
{
    public decimal MinBudget { get; set; }

    public decimal MaxBudget { get; set; }

    public int MinPlayTime { get; set; }

    public int MaxPlayTime { get; set; }

    public int MinPlayers { get; set; }

    public int MaxPlayers { get; set; }

    public List<string> Genres { get; set; } = [];

    public List<string> Types { get; set; } = [];
}

public sealed class AiGameContext
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int MinPlayers { get; set; }

    public int MaxPlayers { get; set; }

    public int PlayTimeMinutes { get; set; }

    public int LearningTimeMinutes { get; set; }

    public int MinimumAge { get; set; }

    public decimal Rating { get; set; }

    public List<string> Genres { get; set; } = [];

    public List<string> Types { get; set; } = [];
}