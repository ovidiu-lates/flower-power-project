using System.Diagnostics.CodeAnalysis;
namespace FlowerPowerGames.Business.Exceptions;

[ExcludeFromCodeCoverage]
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {

    }
}
