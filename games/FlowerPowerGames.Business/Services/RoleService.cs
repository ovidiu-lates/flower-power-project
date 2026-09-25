using AutoMapper;
using AutoMapper.QueryableExtensions;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FlowerPowerGames.Business.Services;

public class RoleService : IRoleService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public RoleService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        return await _context.Roles
            .AsNoTracking()
            .ProjectTo<RoleDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<RoleDto?> GetRoleByIdAsync(int id)
    {
        return await _context.Roles
            .AsNoTracking()
            .Where(role => role.RoleId == id)
            .ProjectTo<RoleDto>(_mapper.ConfigurationProvider)
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
            throw new InvalidOperationException($"A role named '{name}' already exists.");
        }

        var role = _mapper.Map<Role>(roleDto);
        role.Name = name;

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto?> UpdateRoleAsync(int id, RoleDto roleDto)
    {
        if (string.IsNullOrWhiteSpace(roleDto.Name))
        {
            throw new ArgumentException("Role name cannot be empty.");
        }

        var role = await _context.Roles.FirstOrDefaultAsync(item => item.RoleId == id);

        if (role is null)
        {
            return null;
        }

        var name = roleDto.Name.Trim();

        var nameExists = await _context.Roles.AnyAsync(item =>
                item.Name.ToLower() == name.ToLower()
                && item.RoleId != id);

        if (nameExists)
        {
            throw new InvalidOperationException(
                $"A role named '{name}' already exists.");
        }

        _mapper.Map(roleDto, role);
        role.Name = name;

        await _context.SaveChangesAsync();

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(item => item.RoleId == id);

        if (role is null)
        {
            return false;
        }

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        return true;
    }
}