using System.Diagnostics.CodeAnalysis;
namespace FlowerPowerGames.Business.DTOs;
   
[ExcludeFromCodeCoverage]

public sealed class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
