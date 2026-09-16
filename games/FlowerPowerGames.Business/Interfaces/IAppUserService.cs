using System;
using System.Collections.Generic;
using System.Text;
using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IAppUserService
{
    Task<List<AppUserDto>> GetAllUsersAsync();

    Task<AppUserDto?> GetUserByIdAsync(int id);

    Task<AppUserDto> CreateUserAsync(AppUserDto userDto);

    Task<AppUserDto?> UpdateUserAsync(
        int id,
        AppUserDto userDto);

    Task<bool> DeleteUserAsync(int id);
}
