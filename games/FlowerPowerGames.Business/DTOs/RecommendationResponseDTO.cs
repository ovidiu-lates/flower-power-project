using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.DTOs;

public class RecommendationResponseDTO
{
    public List<RecommendedGameDTO> Recommendations { get; set; } = [];
}
