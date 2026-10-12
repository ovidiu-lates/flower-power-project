using System.Diagnostics.CodeAnalysis;
namespace FlowerPowerGames.Business.Exceptions;

[ExcludeFromCodeCoverage]
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
