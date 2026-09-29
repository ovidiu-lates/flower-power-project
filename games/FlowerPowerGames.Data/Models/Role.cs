namespace FlowerPowerGames.Data.Models;

public class Role
{
    public int RoleId { get; set; }

    public required string Name { get; set; }

    public ICollection<User> Users { get; set; } = [];
}