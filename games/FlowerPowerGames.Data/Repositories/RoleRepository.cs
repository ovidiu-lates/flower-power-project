using System;
using System.Collections.Generic;
using System.Text;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerPowerGames.Data.Repositories;

public class RoleRepository(AppDbContext dbContext) : IRoleRepository
{
    public async Task<List<Role>> GetAllAsync()
    {
        return await dbContext.Roles
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        return await dbContext.Roles
            .FirstOrDefaultAsync(role => role.RoleId == id);
    }

    public async Task<Role> AddAsync(Role role)
    {
        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();

        return role;
    }

    public async Task<bool> UpdateAsync(Role role)
    {
        var existingRole = await dbContext.Roles
            .FirstOrDefaultAsync(item => item.RoleId == role.RoleId);

        if (existingRole is null)
        {
            return false;
        }

        existingRole.Name = role.Name;

        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var role = await dbContext.Roles
            .FirstOrDefaultAsync(item => item.RoleId == id);

        if (role is null)
        {
            return false;
        }

        dbContext.Roles.Remove(role);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        int? excludedId = null)
    {
        return await dbContext.Roles.AnyAsync(role =>
            role.Name == name &&
            (!excludedId.HasValue || role.RoleId != excludedId.Value));
    }
}