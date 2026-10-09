using System.Diagnostics.CodeAnalysis;
namespace FlowerPowerGames.Business.DTOs;
    
[ExcludeFromCodeCoverage]

public class UserDto
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public required string Email { get; set; }

    public required string FullName { get; set; }

    public required string Username { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
