using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace FlowerPowerGames.Business.DTOs;
    [ExcludeFromCodeCoverage]

public sealed class RefreshTokenRequestDTO
{
    public string RefreshToken { get; set; } = string.Empty;
}
