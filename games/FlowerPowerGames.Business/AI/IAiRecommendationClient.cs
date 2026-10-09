using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.AI;

public interface IAiRecommendationClient
{
    Task<AiRecommendationResult> GetRecommendationsAsync(RecommendationContext context, 
            int recommendationCount, CancellationToken cancellationToken = default);
}
