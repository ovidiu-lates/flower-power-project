using System.Diagnostics.CodeAnalysis;
namespace FlowerPowerGames.Business.DTOs;

[ExcludeFromCodeCoverage]

public sealed class RegisterRequestDto
{
    public string Email { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string ConfirmPassword { get; set; } = string.Empty;
}
