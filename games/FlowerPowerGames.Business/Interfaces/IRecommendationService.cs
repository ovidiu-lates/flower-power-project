using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IRecommendationService
{
    Task<RecommendationResponseDTO> GetRecommendationsAsync(int userId, RecommendationRequestDTO request, int recommendationCount = 5, CancellationToken cancellationToken = default);
}
