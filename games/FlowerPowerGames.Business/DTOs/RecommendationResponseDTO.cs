using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.DTOs;

public class RecommendationResponseDto
{
    public List<RecommendedGameDto> Recommendations { get; set; } = [];
}
