using System.ComponentModel.DataAnnotations;

namespace FlowerPowerGames.Business.DTOs;

public sealed class UpdateProfileRequestDTO
{
    [EmailAddress]
    public string? Email { get; set; }

    public string? Username { get; set; }

    public string? FullName { get; set; }
}