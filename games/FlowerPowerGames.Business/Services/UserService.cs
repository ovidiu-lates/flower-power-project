using AutoMapper;
using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
namespace FlowerPowerGames.Business.Services;

public sealed class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    public UserService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AuthUser?> FindByEmailOrUsernameAsync(string emailOrUsername)
    {
        var user = await _context.Users
            .Include(user => user.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(user =>
                user.Email == emailOrUsername ||
                user.Username == emailOrUsername);

        return user is null ? null : MapToAuthUser(user);
    }

    public async Task<AuthUser?> FindByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(user => user.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id);

        return user is null ? null : MapToAuthUser(user);
    }

    public async Task<AuthUser> CreateAsync(AuthUser authUser)
    {
        var userRole = await _context.Roles.SingleOrDefaultAsync(role => role.Name == "User");

        if (userRole is null)
        {
            throw new NotFoundException("The User role does not exist in the database.");
        }

        var user = new User
        {
            Email = authUser.Email,
            Username = authUser.Username,
            FullName = authUser.FullName,
            PasswordHash = authUser.PasswordHash,
            IsActive = authUser.IsActive,
            RoleId = userRole.RoleId
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        authUser.Id = user.Id;
        authUser.Role = userRole.Name;

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
            Role = user.Role?.Name ?? "User"
        };
    }
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .AsNoTracking()
            .Include(user => user.Role)
            .ToListAsync();

        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(user => user.Role)
            .FirstOrDefaultAsync(user => user.Id == id);

        if (user is null)
        {
            throw new NotFoundException($"User with id {id} was not found.");
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> UpdateAdminUserAsync(int id, AdminUpdateUserRequestDTO request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

        if (user is null)
        {
            throw new NotFoundException($"User with id {id} was not found.");
        }

        if (request.RoleId.HasValue)
        {
            await EnsureRoleExistsAsync(request.RoleId.Value);
            user.RoleId = request.RoleId.Value;
        }

        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(item => item.Id == id);

        if (user is null)
        {
            throw new NotFoundException($"User with id {id} was not found.");
        }

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return true;
    }

    private async Task EnsureRoleExistsAsync(int roleId)
    {
        var roleExists = await _context.Roles
            .AnyAsync(role => role.RoleId == roleId);

        if (!roleExists)
        {
            throw new NotFoundException(
                $"Role with ID {roleId} does not exist.");
        }
    }

    private async Task EnsureEmailIsUniqueAsync(
        string email,
        int? excludedId = null)
    {
        var normalizedEmail = email.Trim().ToLower();

        var exists = await _context.Users
            .AnyAsync(user =>
                user.Email.ToLower() == normalizedEmail &&
                (!excludedId.HasValue ||
                 user.Id != excludedId.Value));

        if (exists)
        {
            throw new ConflictException(
                $"An account with email '{email}' already exists.");
        }
    }

    private async Task EnsureUsernameIsUniqueAsync(
        string username,
        int? excludedId = null)
    {
        var normalizedUsername = username.Trim().ToLower();

        var exists = await _context.Users
            .AnyAsync(user =>
                user.Username.ToLower() == normalizedUsername &&
                (!excludedId.HasValue ||
                 user.Id != excludedId.Value));

        if (exists)
        {
            throw new ConflictException(
                $"The username '{username}' already exists.");
        }
    }

    public async Task<UserDto?> UpdateMyProfileAsync(int userId, UpdateProfileRequestDTO request)
    {
        if (request.Email is null && request.Username is null && request.FullName is null)
        {
            throw new ArgumentException("At least one field must be provided.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);

        if (user is null)
        {
            return null;
        }

        if (request.Email is not null)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new ArgumentException("Email cannot be empty.");
            }

            var email = request.Email.Trim();

            await EnsureEmailIsUniqueAsync(email, userId);

            user.Email = email;
        }

        if (request.Username is not null)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                throw new ArgumentException("Username cannot be empty.");
            }

            var username = request.Username.Trim();

            await EnsureUsernameIsUniqueAsync(username, userId);

            user.Username = username;
        }

        if (request.FullName is not null)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                throw new ArgumentException("Full name cannot be empty.");
            }

            user.FullName = request.FullName.Trim();
        }

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }
}