using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Business.Authentication;
using Microsoft.AspNetCore.Identity;

namespace FlowerPowerGames.Business.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserService _userStore;
    private readonly IPasswordHasher<AuthUser> _passwordHasher;

    public AuthService(
        IUserService userStore,
        IPasswordHasher<AuthUser> passwordHasher)
    {
        _userStore = userStore;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO request)
    {
        var user = await _userStore.FindByEmailOrUsernameAsync(
            request.EmailOrUsername);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return new LoginResponseDTO
        {
            UserId = user.Id,
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role
        };
    }

    public async Task<RegisterResponseDTO?> RegisterAsync(RegisterRequestDTO request)
    {
        var existingEmail = await _userStore
            .FindByEmailOrUsernameAsync(request.Email);

        if (existingEmail is not null)
        {
            return null;
        }

        var existingUsername = await _userStore
            .FindByEmailOrUsernameAsync(request.Username);

        if (existingUsername is not null)
        {
            return null;
        }

        var user = new AuthUser
        {
            Email = request.Email,
            Username = request.Username,
            FullName = request.FullName,
            Role = "User",
            IsActive = true
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password);

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
}