using FlowerPowerGames.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.Interfaces;

public interface IRecommendationService
{
    Task<RecommendationResponseDTO> GetRecommendationsAsync(int userId, RecommendationRequestDTO request, int recommendationCount = 5, CancellationToken cancellationToken = default);
}
