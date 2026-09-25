using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
using FlowerPowerGames.Business.DTOs;
using AutoMapper;
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
            throw new InvalidOperationException("The User role does not exist in the database.");
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

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(user => user.Role)
            .FirstOrDefaultAsync(user => user.Id == id);

        if (user is null)
        {
            return null;
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateUserAsync(UserDto userDto)
    {
        ValidateUser(userDto);

        await EnsureRoleExistsAsync(userDto.RoleId);
        await EnsureEmailIsUniqueAsync(userDto.Email);
        await EnsureUsernameIsUniqueAsync(userDto.Username);

        var user = _mapper.Map<User>(userDto);

        user.Email = userDto.Email.Trim();
        user.Username = userDto.Username.Trim();
        user.FullName = userDto.FullName.Trim();
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UserDto userDto)
    {
        ValidateUser(userDto);

        var user = await _context.Users
            .FirstOrDefaultAsync(item => item.Id == id);

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

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(item => item.Id == id);

        if (user is null)
        {
            return false;
        }

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return true;
    }

    private static void ValidateUser(UserDto userDto)
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

        var exists = await _context.Users
            .AnyAsync(user =>
                user.Email.ToLower() == normalizedEmail &&
                (!excludedId.HasValue ||
                 user.Id != excludedId.Value));

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

        var exists = await _context.Users
            .AnyAsync(user =>
                user.Username.ToLower() == normalizedUsername &&
                (!excludedId.HasValue ||
                 user.Id != excludedId.Value));

        if (exists)
        {
            throw new InvalidOperationException(
                $"The username '{username}' already exists.");
        }
    }
}