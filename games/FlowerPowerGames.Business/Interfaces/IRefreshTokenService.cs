using FlowerPowerGames.Business.Authentication;

namespace FlowerPowerGames.Business.Interfaces;

public interface IRefreshTokenService
{
    Task SaveAsync(RefreshToken record);

    Task<RefreshToken?> FindAsync(string tokenHash);

    Task RevokeAsync(string tokenHash);
}