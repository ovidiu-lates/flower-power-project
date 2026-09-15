using FlowerPowerGames.Business.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FlowerPowerGames.Data.Models;

namespace FlowerPowerGames.Business.Services
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllAsync();

        Task<Role?> GetByIdAsync(int id);

        Task<Role?> CreateAsync(Role role);

        Task<RoleUpdateResult> UpdateAsync(Role role);

        Task<bool> DeleteAsync(int id);

    }

    public enum RoleUpdateResult
    {
        Success,
        NotFound,
        DuplicateName
    }
}
