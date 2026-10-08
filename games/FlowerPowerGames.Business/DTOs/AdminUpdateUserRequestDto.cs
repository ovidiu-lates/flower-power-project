using System.ComponentModel.DataAnnotations;

public sealed class AdminUpdateUserRequestDTO
{
    [Range(1, int.MaxValue)]
    public int? RoleId { get; set; }

    public bool? IsActive { get; set; }
}