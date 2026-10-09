using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Business.AI;

[ExcludeFromCodeCoverage]
public sealed class AiRecommendationException : Exception
{
    public AiRecommendationException(string message) : base(message)
    {
    }

    public AiRecommendationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
