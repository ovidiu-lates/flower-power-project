using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IRecommendationService
{
    Task<RecommendationResponseDto> GetRecommendationsAsync(int userId, RecommendationRequestDto request, int recommendationCount = 5, CancellationToken cancellationToken = default);
}
