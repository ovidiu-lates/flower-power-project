using System.ComponentModel.DataAnnotations;

namespace FlowerPowerGames.Business.DTOs;

public sealed class LoginRequestDTO
{
    [Required]
    public string EmailOrUsername { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}