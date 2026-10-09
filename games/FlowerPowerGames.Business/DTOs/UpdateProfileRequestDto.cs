using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace FlowerPowerGames.Business.DTOs;
    
[ExcludeFromCodeCoverage]

public sealed class UpdateProfileRequestDto
{
    [EmailAddress]
    public string? Email { get; set; }

    public string? Username { get; set; }

    public string? FullName { get; set; }
}
