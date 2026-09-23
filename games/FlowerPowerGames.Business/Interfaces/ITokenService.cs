using FlowerPowerGames.Business.Authentication;

namespace FlowerPowerGames.Business.Interfaces;

public interface ITokenService
{
    TokenPair CreateTokenPair(AuthUser user);

    string HashRefreshToken(string refreshToken);

    Task SaveAsync(RefreshToken refreshToken);

    Task<RefreshToken?> FindAsync(string tokenHash);

    Task RevokeAsync(string tokenHash);
}