using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
