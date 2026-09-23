using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerPowerGames.Business.Services;

public sealed class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AuthUser?> FindByEmailOrUsernameAsync(string emailOrUsername)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == emailOrUsername || user.Username == emailOrUsername);

        return user is null ? null : MapToAuthUser(user);
    }

    public async Task<AuthUser?> FindByIdAsync(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id);

        return user is null ? null : MapToAuthUser(user);
    }

    public async Task<AuthUser> CreateAsync(AuthUser authUser)
    {
        var user = new User
        {
            Email = authUser.Email,
            Username = authUser.Username,
            FullName = authUser.FullName,
            PasswordHash = authUser.PasswordHash,
            IsActive = authUser.IsActive
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        authUser.Id = user.Id;

        return authUser;
    }

    private static AuthUser MapToAuthUser(User user)
    {
        return new AuthUser
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName,
            PasswordHash = user.PasswordHash,
            IsActive = user.IsActive,
            Role = "User"
        };
    }
}