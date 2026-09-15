using FlowerPowerGames.Data.Models;

namespace FlowerPowerGames.Data.Repositories;

public interface IRoleRepository
{
    Task<List<Role>> GetAllAsync();

    Task<Role?> GetByIdAsync(int id);

    Task<Role> AddAsync(Role role);

    Task<bool> UpdateAsync(Role role);

    Task<bool> DeleteAsync(int id);

    Task<bool> ExistsByNameAsync(string name, int? excludedId = null);
}