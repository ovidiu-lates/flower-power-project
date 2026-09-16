using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerPowerGames.Business.Services;

public class RoleService : IRoleService
{
    private readonly AppDbContext _context;

    public RoleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        return await _context.Roles
            .AsNoTracking()
            .Select(role => new RoleDto
            {
                RoleId = role.RoleId,
                Name = role.Name
            })
            .ToListAsync();
    }

    public async Task<RoleDto?> GetRoleByIdAsync(int id)
    {
        return await _context.Roles
            .AsNoTracking()
            .Where(role => role.RoleId == id)
            .Select(role => new RoleDto
            {
                RoleId = role.RoleId,
                Name = role.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task<RoleDto> CreateRoleAsync(RoleDto roleDto)
    {
        if (string.IsNullOrWhiteSpace(roleDto.Name))
        {
            throw new ArgumentException("Role name cannot be empty.");
        }

        var name = roleDto.Name.Trim();

        var nameExists = await _context.Roles
            .AnyAsync(role => role.Name.ToLower() == name.ToLower());

        if (nameExists)
        {
            throw new InvalidOperationException(
                $"A role named '{name}' already exists.");
        }

        var role = new Role
        {
            Name = name
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return new RoleDto
        {
            RoleId = role.RoleId,
            Name = role.Name
        };
    }

    public async Task<RoleDto?> UpdateRoleAsync(int id, RoleDto roleDto)
    {
        if (string.IsNullOrWhiteSpace(roleDto.Name))
        {
            throw new ArgumentException("Role name cannot be empty.");
        }

        var role = await _context.Roles
            .FirstOrDefaultAsync(item => item.RoleId == id);

        if (role is null)
        {
            return null;
        }

        var name = roleDto.Name.Trim();

        var nameExists = await _context.Roles
            .AnyAsync(item =>
                item.Name.ToLower() == name.ToLower() &&
                item.RoleId != id);

        if (nameExists)
        {
            throw new InvalidOperationException(
                $"A role named '{name}' already exists.");
        }

        role.Name = name;

        await _context.SaveChangesAsync();

        return new RoleDto
        {
            RoleId = role.RoleId,
            Name = role.Name
        };
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(item => item.RoleId == id);

        if (role is null)
        {
            return false;
        }

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        return true;
    }
}