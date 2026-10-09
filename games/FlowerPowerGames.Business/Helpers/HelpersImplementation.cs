using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Business.Helpers;

[ExcludeFromCodeCoverage]
public static class HelpersImplementation
{
    public static string CleanName(string name)
    {
        return string.Join(" ", name.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
