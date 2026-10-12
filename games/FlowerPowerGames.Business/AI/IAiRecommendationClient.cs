namespace FlowerPowerGames.Business.AI;

public interface IAiRecommendationClient
{
    Task<AiRecommendationResult> GetRecommendationsAsync(RecommendationContext context, 
            int recommendationCount, CancellationToken cancellationToken = default);
}
