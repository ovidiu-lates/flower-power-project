using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.Exceptions;

public class AiUsageLimitExceededException : Exception
{
    public AiUsageLimitExceededException(string message) : base(message)
    {

    }
}
