using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FlowerPowerGames.Business.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserService _userStore;
    private readonly IPasswordHasher<AuthUser> _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(IUserService userStore, IPasswordHasher<AuthUser> passwordHasher, ITokenService tokenService)
    {
        _userStore = userStore;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
    {
        var user = await _userStore.FindByEmailOrUsernameAsync(request.EmailOrUsername);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedException("Invalid username/email or password.");
        }

        var passwordResult =_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException("Invalid username/email or password.");
        }

        return await CreateLoginResponseAsync(user);
    }

    public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request)
    {
        var existingEmail = await _userStore.FindByEmailOrUsernameAsync(request.Email);

        if (existingEmail is not null)
        {
            throw new ConflictException("Email or username already exists.");
        }

        var existingUsername = await _userStore.FindByEmailOrUsernameAsync(request.Username);

        if (existingUsername is not null)
        {
            throw new ConflictException("Email or username already exists.");
        }

        var user = new AuthUser
        {
            Email = request.Email,
            Username = request.Username,
            FullName = request.FullName,
            Role = "User",
            IsActive = true
        };

        user.PasswordHash =_passwordHasher.HashPassword(user, request.Password);

        var createdUser = await _userStore.CreateAsync(user);

        return new RegisterResponseDTO
        {
            UserId = createdUser.Id,
            Email = createdUser.Email,
            Username = createdUser.Username,
            FullName = createdUser.FullName,
            Role = createdUser.Role
        };
    }

    public async Task<LoginResponseDTO> RefreshAsync(string refreshToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(refreshToken);

        var storedToken = await _tokenService.FindAsync(tokenHash);

        if (storedToken is null || storedToken.RevokedAtUtc is not null || storedToken.ExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        var user = await _userStore.FindByIdAsync(storedToken.UserId);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        await _tokenService.RevokeAsync(tokenHash);

        return await CreateLoginResponseAsync(user);
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(refreshToken);

        var storedToken = await _tokenService.FindAsync(tokenHash);

        if (storedToken is null)
        {
            return false;
        }

        await _tokenService.RevokeAsync(tokenHash);

        return true;
    }

    private async Task<LoginResponseDTO> CreateLoginResponseAsync(AuthUser user)
    {
        var tokens = _tokenService.CreateTokenPair(user);

        await _tokenService.SaveAsync(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = _tokenService.HashRefreshToken(tokens.RefreshToken),
                ExpiresAtUtc = tokens.RefreshTokenExpiresAtUtc
            });

        return new LoginResponseDTO
        {
            UserId = user.Id,
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            AccessTokenExpiresAtUtc = tokens.AccessTokenExpiresAtUtc
        };
    }
}