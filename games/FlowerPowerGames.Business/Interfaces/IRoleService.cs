using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IRoleService
{
    Task<List<RoleDto>> GetAllRolesAsync();

    Task<RoleDto?> GetRoleByIdAsync(int id);

    Task<RoleDto> CreateRoleAsync(RoleDto roleDto);

    Task<RoleDto?> UpdateRoleAsync(int id, RoleDto roleDto);

    Task<bool> DeleteRoleAsync(int id);
}