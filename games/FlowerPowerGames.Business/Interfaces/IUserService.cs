using FlowerPowerGames.Business.Authentication;

namespace FlowerPowerGames.Business.Interfaces;

public interface IUserService
{
    Task<AuthUser?> FindByEmailOrUsernameAsync(string emailOrUsername);

    Task<AuthUser?> FindByIdAsync(int id);

    Task<AuthUser> CreateAsync(AuthUser user);
}