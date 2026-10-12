using System.Diagnostics.CodeAnalysis;
namespace FlowerPowerGames.Business.DTOs;
    
[ExcludeFromCodeCoverage]

public sealed class RegisterResponseDto
{
    public int UserId { get; init; }

    public string Email { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;
}
