using System.Diagnostics.CodeAnalysis;
namespace FlowerPowerGames.Business.DTOs;
   
[ExcludeFromCodeCoverage]

public class RoleDto
{
    public int RoleId { get; set; }

    public required string Name { get; set; }
}
