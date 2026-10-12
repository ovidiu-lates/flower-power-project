using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Business.DTOs;

[ExcludeFromCodeCoverage]
public sealed class LoginRequestDto
{
    public string EmailOrUsername { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
