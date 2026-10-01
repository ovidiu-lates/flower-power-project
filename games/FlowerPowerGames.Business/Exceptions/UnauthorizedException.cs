using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {

    }
}
