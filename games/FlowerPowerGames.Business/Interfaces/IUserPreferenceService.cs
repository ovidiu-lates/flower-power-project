using System;
using System.Collections.Generic;
using System.Text;
using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IUserPreferenceService
{
    Task<UserPreferenceDTO> GetUserPreferenceByUserIdAsync(int userId);
    Task<UserPreferenceDTO> CreateUserPreferenceAsync(CreateUserPreferenceDTO userPreferenceDto, int userId);
    Task<UserPreferenceDTO> UpdateUserPreferenceAsync(int userId, CreateUserPreferenceDTO userPreferenceDto);
}
