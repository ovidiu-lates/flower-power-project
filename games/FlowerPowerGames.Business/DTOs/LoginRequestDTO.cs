using System.ComponentModel.DataAnnotations;

namespace FlowerPowerGames.Business.DTOs;

public sealed class LoginRequestDTO
{
    public string EmailOrUsername { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}