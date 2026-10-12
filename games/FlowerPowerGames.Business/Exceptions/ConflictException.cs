using System.Diagnostics.CodeAnalysis;
namespace FlowerPowerGames.Business.Exceptions;

[ExcludeFromCodeCoverage]
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
