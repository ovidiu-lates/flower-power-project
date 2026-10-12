using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Business.DTOs;

[ExcludeFromCodeCoverage]

public class RecommendationRequestDto
{
    public string Message { get; set; } = string.Empty;
}
