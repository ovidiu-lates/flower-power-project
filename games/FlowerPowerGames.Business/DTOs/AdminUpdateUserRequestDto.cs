using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Business.DTOs;

[ExcludeFromCodeCoverage]
public sealed class AdminUpdateUserRequestDto
{
    public int? RoleId { get; set; }

    public bool? IsActive { get; set; }
}
