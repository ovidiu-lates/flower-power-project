using AutoMapper;
using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Mappers;
using FlowerPowerGames.Business.Services;
using FlowerPowerGames.Business.Validators;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FlowerPowerGames.UnitTests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task FindByEmailOrUsernameAsync_UserForEmailOrUsername()
    {
        using var context = CreateContext();
        context.Users.Add(CreateUser());
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var byEmail = await service.FindByEmailOrUsernameAsync("jane@example.com");
        var byUsername = await service.FindByEmailOrUsernameAsync("jane");

        Assert.NotNull(byEmail);
        Assert.Equal(1, byEmail.Id);
        Assert.Equal("User", byEmail.Role);
        Assert.NotNull(byUsername);
        Assert.Equal(byEmail.Id, byUsername.Id);
    }

    [Fact]
    public async Task FindByEmailOrUsernameAsync_ReturnsNull_UserDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var user = await service.FindByEmailOrUsernameAsync("missing@example.com");

        Assert.Null(user);
    }

    [Fact]
    public async Task FindByIdAsync_ReturnsMappedUser_UserExists()
    {
        using var context = CreateContext();
        context.Users.Add(CreateUser());
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var user = await service.FindByIdAsync(1);

        Assert.NotNull(user);
        Assert.Equal("jane@example.com", user.Email);
        Assert.Equal("jane", user.Username);
        Assert.Equal("Jane Doe", user.FullName);
        Assert.Equal("User", user.Role);
    }

    [Fact]
    public async Task FindByIdAsync_ReturnsNull_UserDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var user = await service.FindByIdAsync(42);

        Assert.Null(user);
    }

    [Fact]
    public async Task CreateAsync_AssignsUserRoleAndPersistsUser()
    {
        using var context = CreateContext();
        var service = CreateService(context);
        var authUser = new AuthUser
        {
            Email = "new@example.com",
            Username = "newuser",
            FullName = "New User",
            PasswordHash = "password-hash"
        };

        var createdUser = await service.CreateAsync(authUser);

        Assert.True(createdUser.Id > 0);
        Assert.Equal("User", createdUser.Role);
        var storedUser = await context.Users.SingleAsync();
        Assert.Equal("new@example.com", storedUser.Email);
        Assert.Equal("newuser", storedUser.Username);
        Assert.Equal(1, storedUser.RoleId);
    }

    [Fact]
    public async Task CreateAsync_ThrowsNotFoundException_UserRoleDoesNotExist()
    {
        using var context = CreateContext();
        var userRole = await context.Roles.SingleAsync(role => role.Name == "User");
        context.Roles.Remove(userRole);
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.CreateAsync(new AuthUser { Email = "new@example.com", Username = "newuser", FullName = "New User" }));

        Assert.Equal("The User role does not exist in the database.", exception.Message);
        Assert.Empty(context.Users);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsMappedUsers()
    {
        using var context = CreateContext();
        context.Users.AddRange(
            CreateUser(1, "jane@example.com", "jane", "Jane Doe"),
            CreateUser(2, "john@example.com", "john", "John Doe", roleId: 2));
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var users = await service.GetAllUsersAsync();

        Assert.Collection(
            users,
            user =>
            {
                Assert.Equal(1, user.Id);
                Assert.Equal("Jane Doe", user.FullName);
                Assert.Equal(1, user.RoleId);
            },
            user =>
            {
                Assert.Equal(2, user.Id);
                Assert.Equal("John Doe", user.FullName);
                Assert.Equal(2, user.RoleId);
            });
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsMappedUser_UserExists()
    {
        using var context = CreateContext();
        context.Users.Add(CreateUser());
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var user = await service.GetUserByIdAsync(1);

        Assert.Equal(1, user.Id);
        Assert.Equal("jane@example.com", user.Email);
        Assert.Equal("jane", user.Username);
        Assert.Equal(1, user.RoleId);
    }

    [Fact]
    public async Task GetUserByIdAsync_ThrowsNotFoundException_UserDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => service.GetUserByIdAsync(42));

        Assert.Equal("User with id 42 was not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateAdminUserAsync_UpdatesRoleAndActiveState()
    {
        using var context = CreateContext();
        var user = CreateUser();
        user.UpdatedAt = DateTime.UtcNow.AddDays(-1);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var updatedUser = await service.UpdateAdminUserAsync(1, new AdminUpdateUserRequestDto
        {
            RoleId = 2,
            IsActive = false
        });

        Assert.NotNull(updatedUser);
        Assert.Equal(2, updatedUser.RoleId);
        Assert.False(updatedUser.IsActive);
        var storedUser = await context.Users.SingleAsync();
        Assert.Equal(2, storedUser.RoleId);
        Assert.False(storedUser.IsActive);
        Assert.True(storedUser.UpdatedAt > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task UpdateAdminUserAsync_ThrowsNotFoundException_UserDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.UpdateAdminUserAsync(42, new AdminUpdateUserRequestDto { IsActive = false }));

        Assert.Equal("User with id 42 was not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateAdminUserAsync_ThrowsNotFoundException_RoleDoesNotExist()
    {
        using var context = CreateContext();
        context.Users.Add(CreateUser());
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.UpdateAdminUserAsync(1, new AdminUpdateUserRequestDto { RoleId = 99 }));

        Assert.Equal("Role with ID 99 does not exist.", exception.Message);
        Assert.Equal(1, (await context.Users.SingleAsync()).RoleId);
    }

    [Fact]
    public async Task DeleteUserAsync_RemovesUserAndReturnsTrue()
    {
        using var context = CreateContext();
        context.Users.Add(CreateUser());
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var deleted = await service.DeleteUserAsync(1);

        Assert.True(deleted);
        Assert.Empty(context.Users);
    }

    [Fact]
    public async Task DeleteUserAsync_ThrowsNotFoundException_UserDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteUserAsync(42));

        Assert.Equal("User with id 42 was not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateMyProfileAsync_TrimsAndPersistsProfileFields()
    {
        using var context = CreateContext();
        context.Users.Add(CreateUser());
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var updatedUser = await service.UpdateMyProfileAsync(1, new UpdateProfileRequestDto
        {
            Email = "  jane.updated@example.com  ",
            Username = "  janedoe  ",
            FullName = "  Jane Updated  "
        });

        Assert.NotNull(updatedUser);
        Assert.Equal("jane.updated@example.com", updatedUser.Email);
        Assert.Equal("janedoe", updatedUser.Username);
        Assert.Equal("Jane Updated", updatedUser.FullName);
        var storedUser = await context.Users.SingleAsync();
        Assert.Equal(updatedUser.Email, storedUser.Email);
        Assert.Equal(updatedUser.Username, storedUser.Username);
        Assert.Equal(updatedUser.FullName, storedUser.FullName);
    }

    [Fact]
    public async Task UpdateMyProfileAsync_ThrowsNotFoundException_UserDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.UpdateMyProfileAsync(42, new UpdateProfileRequestDto { FullName = "New Name" }));

        Assert.Equal("User with id 42 was not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateMyProfileAsync_ThrowsConflictException_EmailIsAlreadyUsed()
    {
        using var context = CreateContext();
        context.Users.AddRange(
            CreateUser(1, "jane@example.com", "jane", "Jane Doe"),
            CreateUser(2, "john@example.com", "john", "John Doe"));
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ConflictException>(() =>
            service.UpdateMyProfileAsync(2, new UpdateProfileRequestDto { Email = "JANE@example.com" }));

        Assert.Equal("An account with email 'JANE@example.com' already exists.", exception.Message);
        Assert.Equal("john@example.com", (await context.Users.SingleAsync(user => user.Id == 2)).Email);
    }

    [Fact]
    public async Task UpdateMyProfileAsync_ThrowsConflictException_UsernameIsAlreadyUsed()
    {
        using var context = CreateContext();
        context.Users.AddRange(
            CreateUser(1, "jane@example.com", "jane", "Jane Doe"),
            CreateUser(2, "john@example.com", "john", "John Doe"));
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ConflictException>(() =>
            service.UpdateMyProfileAsync(2, new UpdateProfileRequestDto { Username = "JANE" }));

        Assert.Equal("The username 'JANE' already exists.", exception.Message);
        Assert.Equal("john", (await context.Users.SingleAsync(user => user.Id == 2)).Username);
    }

    [Fact]
    public async Task ChangePasswordAsync_UpdatesPasswordHash_CurrentPasswordIsCorrect()
    {
        using var context = CreateContext();
        var hasher = new PasswordHasher<AuthUser>();
        var authUser = new AuthUser { Id = 1, Email = "jane@example.com", Username = "jane", FullName = "Jane Doe" };
        var user = CreateUser();
        user.PasswordHash = hasher.HashPassword(authUser, "OldPassword123!");
        var originalPasswordHash = user.PasswordHash;
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = CreateService(context, hasher);

        var changed = await service.ChangePasswordAsync(1, new ChangePasswordRequestDto
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword456!",
            ConfirmNewPassword = "NewPassword456!"
        });

        Assert.True(changed);
        var updatedUser = await context.Users.SingleAsync();
        Assert.NotEqual(originalPasswordHash, updatedUser.PasswordHash);
        var verificationUser = new AuthUser { Id = 1, Email = user.Email, Username = user.Username, FullName = user.FullName };
        Assert.Equal(PasswordVerificationResult.Success, hasher.VerifyHashedPassword(verificationUser, updatedUser.PasswordHash, "NewPassword456!"));
    }

    [Fact]
    public async Task ChangePasswordAsync_ThrowsArgumentException_CurrentPasswordIsIncorrect()
    {
        using var context = CreateContext();
        var hasher = new PasswordHasher<AuthUser>();
        var authUser = new AuthUser { Id = 1, Email = "jane@example.com", Username = "jane", FullName = "Jane Doe" };
        var user = CreateUser();
        user.PasswordHash = hasher.HashPassword(authUser, "OldPassword123!");
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = CreateService(context, hasher);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ChangePasswordAsync(1, new ChangePasswordRequestDto
            {
                CurrentPassword = "WrongPassword!",
                NewPassword = "NewPassword456!"
            }));

        Assert.Equal("The current password is incorrect.", exception.Message);
    }

    [Fact]
    public async Task ChangePasswordAsync_ThrowsNotFoundException_UserDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.ChangePasswordAsync(42, new ChangePasswordRequestDto()));

        Assert.Equal("User with id 42 was not found.", exception.Message);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private static UserService CreateService(AppDbContext context, IPasswordHasher<AuthUser>? passwordHasher = null)
    {
        var configuration = new MapperConfiguration(
            config => config.AddProfile<UserProfile>(),
            NullLoggerFactory.Instance);
        return new UserService(
            context,
            configuration.CreateMapper(),
            new AdminUpdateUserRequestDtoValidator(),
            new UpdateProfileRequestDtoValidator(),
            passwordHasher ?? new PasswordHasher<AuthUser>());
    }

    private static User CreateUser(
        int id = 1,
        string email = "jane@example.com",
        string username = "jane",
        string fullName = "Jane Doe",
        int roleId = 1)
    {
        return new User
        {
            Id = id,
            Email = email,
            Username = username,
            FullName = fullName,
            PasswordHash = "password-hash",
            RoleId = roleId
        };
    }
}
