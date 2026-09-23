using FlowerPowerGames.Business.Authentication;

namespace FlowerPowerGames.Business.Interfaces;

public interface ITokenService
{
    TokenPair CreateTokenPair(AuthUser user);

    string HashRefreshToken(string refreshToken);
}