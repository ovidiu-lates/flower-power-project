using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
namespace FlowerPowerGames.Business.Interfaces;

public interface IUserService
{
    Task<AuthUser?> FindByEmailOrUsernameAsync(string emailOrUsername);

    Task<AuthUser?> FindByIdAsync(int id);

    Task<AuthUser> CreateAsync(AuthUser user);

    Task<List<UserDto>> GetAllUsersAsync();

    Task<UserDto> GetUserByIdAsync(int id);

    Task<UserDto?> UpdateAdminUserAsync(int id, AdminUpdateUserRequestDTO request);

    Task<bool> DeleteUserAsync(int id);

    Task<UserDto?> UpdateMyProfileAsync(int userId,UpdateProfileRequestDTO request);

}