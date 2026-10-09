using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public sealed class AdminUpdateUserRequestDTO
{
    [Range(1, int.MaxValue)]
    public int? RoleId { get; set; }

    public bool? IsActive { get; set; }
}
