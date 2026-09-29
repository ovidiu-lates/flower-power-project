using System.ComponentModel.DataAnnotations;

namespace FlowerPowerGames.Business.DTOs;

public sealed class RefreshTokenRequestDTO
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}