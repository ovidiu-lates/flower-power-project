using FlowerPowerGames.Business.Authentication;

namespace FlowerPowerGames.Business.Interfaces;

public interface IUserService
{
    Task<AuthUser?> FindByEmailOrUsernameAsync(string emailOrUsername);
}