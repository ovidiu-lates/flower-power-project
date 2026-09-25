using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
namespace FlowerPowerGames.Business.Interfaces;

public interface IUserService
{
    Task<AuthUser?> FindByEmailOrUsernameAsync(string emailOrUsername);

    Task<AuthUser?> FindByIdAsync(int id);

    Task<AuthUser> CreateAsync(AuthUser user);

    Task<List<UserDto>> GetAllUsersAsync();

    Task<UserDto?> GetUserByIdAsync(int id);

    Task<UserDto> CreateUserAsync(UserDto userDto);

    Task<UserDto?> UpdateUserAsync(
        int id,
        UserDto userDto);

    Task<bool> DeleteUserAsync(int id);

}