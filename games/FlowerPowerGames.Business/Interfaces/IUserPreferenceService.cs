using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IUserPreferenceService
{
    Task<UserPreferenceDto> GetUserPreferenceByUserIdAsync(int userId);
    Task<UserPreferenceDto> CreateUserPreferenceAsync(CreateUserPreferenceDto createUserPreferenceDto, int userId);
    Task<UserPreferenceDto> UpdateUserPreferenceAsync(int userId, CreateUserPreferenceDto userPreferenceDto);
}
