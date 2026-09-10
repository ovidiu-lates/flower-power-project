using FlowerPowerGames.Data.Models;
using FlowerPowerGames.Data.Repositories;

namespace FlowerPowerGames.Business.Services;

public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    public async Task<List<Role>> GetAllAsync()
    {
        return await roleRepository.GetAllAsync();
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        return await roleRepository.GetByIdAsync(id);
    }

    public async Task<Role?> CreateAsync(Role role)
    {
        var nameExists = await roleRepository.ExistsByNameAsync(role.Name);

        if (nameExists)
        {
            return null;
        }

        return await roleRepository.AddAsync(role);
    }

    public async Task<RoleUpdateResult> UpdateAsync(Role role)
    {
        var existingRole = await roleRepository.GetByIdAsync(role.RoleId);

        if (existingRole is null)
        {
            return RoleUpdateResult.NotFound;
        }

        var nameExists = await roleRepository.ExistsByNameAsync(
            role.Name,
            role.RoleId);

        if (nameExists)
        {
            return RoleUpdateResult.DuplicateName;
        }

        await roleRepository.UpdateAsync(role);

        return RoleUpdateResult.Success;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await roleRepository.DeleteAsync(id);
    }
}