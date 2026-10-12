using System.Diagnostics.CodeAnalysis;
namespace FlowerPowerGames.Business.Exceptions;

[ExcludeFromCodeCoverage]
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}
