namespace FlowerPowerGames.Business.DTOs;

public sealed class LoginResponseDTO
{
    public int UserId { get; init; }

    public string Email { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;
}