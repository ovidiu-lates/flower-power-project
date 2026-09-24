using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerPowerGames.Business.Services;

public class AppUserService : IAppUserService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AppUserService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<AppUserDto>> GetAllUsersAsync()
    {
        var users = await _context.AppUsers
            .AsNoTracking()
            .Include(user => user.Role)
            .ToListAsync();

        return _mapper.Map<List<AppUserDto>>(users);
    }

    public async Task<AppUserDto?> GetUserByIdAsync(int id)
    {
        var user = await _context.AppUsers
            .AsNoTracking()
            .Include(user => user.Role)
            .FirstOrDefaultAsync(user => user.UserId == id);

        if (user is null)
        {
            return null;
        }

        return _mapper.Map<AppUserDto>(user);
    }

    public async Task<AppUserDto> CreateUserAsync(AppUserDto userDto)
    {
        ValidateUser(userDto);

        await EnsureRoleExistsAsync(userDto.RoleId);
        await EnsureEmailIsUniqueAsync(userDto.Email);
        await EnsureUsernameIsUniqueAsync(userDto.Username);

        var user = _mapper.Map<AppUser>(userDto);

        user.Email = userDto.Email.Trim();
        user.Username = userDto.Username.Trim();
        user.FullName = userDto.FullName.Trim();
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        _context.AppUsers.Add(user);

        await _context.SaveChangesAsync();

        return _mapper.Map<AppUserDto>(user);
    }

    public async Task<AppUserDto?> UpdateUserAsync( int id, AppUserDto userDto)
    {
        ValidateUser(userDto);

        var user = await _context.AppUsers
            .FirstOrDefaultAsync(item => item.UserId == id);

        if (user is null)
        {
            return null;
        }

        await EnsureRoleExistsAsync(userDto.RoleId);
        await EnsureEmailIsUniqueAsync(userDto.Email, id);
        await EnsureUsernameIsUniqueAsync(userDto.Username, id);

        var createdAt = user.CreatedAt;

        _mapper.Map(userDto, user);

        user.Email = userDto.Email.Trim();
        user.Username = userDto.Username.Trim();
        user.FullName = userDto.FullName.Trim();
        user.CreatedAt = createdAt;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<AppUserDto>(user);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.AppUsers
            .FirstOrDefaultAsync(item => item.UserId == id);

        if (user is null)
        {
            return false;
        }

        _context.AppUsers.Remove(user);

        await _context.SaveChangesAsync();

        return true;
    }

    private static void ValidateUser(AppUserDto userDto)
    {
        if (string.IsNullOrWhiteSpace(userDto.Email))
        {
            throw new ArgumentException("Email cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(userDto.PasswordHash))
        {
            throw new ArgumentException(
                "Password hash cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(userDto.FullName))
        {
            throw new ArgumentException(
                "Full name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(userDto.Username))
        {
            throw new ArgumentException("Username cannot be empty.");
        }

        if (userDto.RoleId <= 0)
        {
            throw new ArgumentException(
                "Role ID must be a positive number.");
        }
    }

    private async Task EnsureRoleExistsAsync(int roleId)
    {
        var roleExists = await _context.Roles
            .AnyAsync(role => role.RoleId == roleId);

        if (!roleExists)
        {
            throw new ArgumentException(
                $"Role with ID {roleId} does not exist.");
        }
    }

    private async Task EnsureEmailIsUniqueAsync(
        string email,
        int? excludedId = null)
    {
        var normalizedEmail = email.Trim().ToLower();

        var exists = await _context.AppUsers
            .AnyAsync(user =>
                user.Email.ToLower() == normalizedEmail &&
                (!excludedId.HasValue ||
                 user.UserId != excludedId.Value));

        if (exists)
        {
            throw new InvalidOperationException(
                $"An account with email '{email}' already exists.");
        }
    }

    private async Task EnsureUsernameIsUniqueAsync(
        string username,
        int? excludedId = null)
    {
        var normalizedUsername = username.Trim().ToLower();

        var exists = await _context.AppUsers
            .AnyAsync(user =>
                user.Username.ToLower() == normalizedUsername &&
                (!excludedId.HasValue ||
                 user.UserId != excludedId.Value));

        if (exists)
        {
            throw new InvalidOperationException(
                $"The username '{username}' already exists.");
        }
    }
}
