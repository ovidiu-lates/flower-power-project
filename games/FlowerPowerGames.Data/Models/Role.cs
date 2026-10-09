using System.Diagnostics.CodeAnalysis;

namespace FlowerPowerGames.Data.Models;

[ExcludeFromCodeCoverage]
public class Role
{
    public int RoleId { get; set; }

    public required string Name { get; set; }

    public ICollection<User> Users { get; set; } = [];
}