using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.AI;

public sealed class AiRecommendationException : Exception
{
    public AiRecommendationException(string message) : base(message)
    {
    }

    public AiRecommendationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
