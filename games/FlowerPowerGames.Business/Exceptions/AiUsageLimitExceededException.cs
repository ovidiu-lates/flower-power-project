using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;
namespace FlowerPowerGames.Business.Exceptions;

[ExcludeFromCodeCoverage]
public class AiUsageLimitExceededException : Exception
{
    public AiUsageLimitExceededException(string message) : base(message)
    {

    }
}
