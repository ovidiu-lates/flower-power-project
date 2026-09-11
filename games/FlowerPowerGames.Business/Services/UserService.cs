using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FlowerPowerGames.Business.Services;

public sealed class UserService : IUserService
{
    private readonly List<AuthUser> _users;

    public UserService( IPasswordHasher<AuthUser> passwordHasher)
    {
        var user = new AuthUser
        {
            Id = 1,
            Email = "user@test.com",
            Username = "testuser",
            FullName = "Test User",
            Role = "User"
        };

        user.PasswordHash = passwordHasher.HashPassword(
            user,
            "Password123!");

        var admin = new AuthUser
        {
            Id = 2,
            Email = "admin@test.com",
            Username = "admin",
            FullName = "Test Admin",
            Role = "Admin"
        };

        admin.PasswordHash = passwordHasher.HashPassword(
            admin,
            "Admin123!");

        _users = [user, admin];
    }

    public Task<AuthUser?> FindByEmailOrUsernameAsync(
        string emailOrUsername)
    {
        var user = _users.FirstOrDefault(user =>
            user.Email.Equals(
                emailOrUsername,
                StringComparison.OrdinalIgnoreCase)
            ||
            user.Username.Equals(
                emailOrUsername,
                StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }
}