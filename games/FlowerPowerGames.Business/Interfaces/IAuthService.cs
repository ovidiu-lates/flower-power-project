using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);

    Task<LoginResponseDto> RefreshAsync(string refreshToken);

    Task<bool> LogoutAsync(string refreshToken);
}