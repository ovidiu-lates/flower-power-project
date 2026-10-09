using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Mappers;
using FlowerPowerGames.Business.Services;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FlowerPowerGames.UnitTests.Services;

public class RoleServiceTests
{
    [Fact]
    public async Task GetAllRolesAsync_ReturnMappedRoles_RolesExist()
    {
        using var context = CreateContext();
        context.Roles.AddRange(
            new Role { RoleId = 1, Name = "Admin" },
            new Role { RoleId = 2, Name = "User" });
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var roles = await service.GetAllRolesAsync();

        Assert.Collection(
            roles,
            role =>
            {
                Assert.Equal(1, role.RoleId);
                Assert.Equal("Admin", role.Name);
            },
            role =>
            {
                Assert.Equal(2, role.RoleId);
                Assert.Equal("User", role.Name);
            });
    }

    [Fact]
    public async Task GetRoleByIdAsync_ReturnMappedRole_RoleExists()
    {
        using var context = CreateContext();
        context.Roles.Add(new Role { RoleId = 3, Name = "Moderator" });
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var role = await service.GetRoleByIdAsync(3);

        Assert.Equal(3, role.RoleId);
        Assert.Equal("Moderator", role.Name);
    }

    [Fact]
    public async Task GetRoleByIdAsync_ThrowNotFoundException_RoleDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetRoleByIdAsync(42));

        Assert.Equal("Role with id 42 was not found.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateRoleAsync_ThrowArgumentException_NameIsNullOrWhitespace(string? name)
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateRoleAsync(new RoleDto { Name = name! }));

        Assert.Equal("Role name cannot be empty.", exception.Message);
        Assert.Empty(context.Roles);
    }

    [Fact]
    public async Task CreateRoleAsync_ThrowConflictException_NameAlreadyExistsIgnoringCase()
    {
        using var context = CreateContext();
        context.Roles.Add(new Role { Name = "Admin" });
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.CreateRoleAsync(new RoleDto { Name = "  aDmIn  " }));

        Assert.Equal("A role named 'aDmIn' already exists.", exception.Message);
        Assert.Single(context.Roles);
    }

    [Fact]
    public async Task CreateRoleAsync_TrimAndPersistName_NameIsUnique()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var createdRole = await service.CreateRoleAsync(new RoleDto { Name = "  Moderator  " });

        Assert.True(createdRole.RoleId > 0);
        Assert.Equal("Moderator", createdRole.Name);
        var storedRole = await context.Roles.SingleAsync();
        Assert.Equal(createdRole.RoleId, storedRole.RoleId);
        Assert.Equal("Moderator", storedRole.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateRoleAsync_ThrowArgumentException_NameIsNullOrWhitespace(string? name)
    {
        using var context = CreateContext();
        context.Roles.Add(new Role { RoleId = 1, Name = "Admin" });
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateRoleAsync(1, new RoleDto { Name = name! }));

        Assert.Equal("Role name cannot be empty.", exception.Message);
        Assert.Equal("Admin", (await context.Roles.SingleAsync()).Name);
    }

    [Fact]
    public async Task UpdateRoleAsync_ThrowNotFoundException_RoleDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateRoleAsync(42, new RoleDto { Name = "Moderator" }));

        Assert.Equal("Role with id 42 was not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateRoleAsync_ThrowConflictException_AnotherRoleHasNameIgnoringCase()
    {
        using var context = CreateContext();
        context.Roles.AddRange(
            new Role { RoleId = 1, Name = "Admin" },
            new Role { RoleId = 2, Name = "User" });
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.UpdateRoleAsync(2, new RoleDto { Name = "  aDmIn  " }));

        Assert.Equal("A role named 'aDmIn' already exists.", exception.Message);
        Assert.Equal("User", (await context.Roles.SingleAsync(role => role.RoleId == 2)).Name);
    }

    [Fact]
    public async Task UpdateRoleAsync_TrimAndPersistName_NameIsUnique()
    {
        using var context = CreateContext();
        context.Roles.Add(new Role { RoleId = 1, Name = "Admin" });
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var updatedRole = await service.UpdateRoleAsync(
            1,
            new RoleDto { Name = "  Moderator  " });

        Assert.Equal(1, updatedRole.RoleId);
        Assert.Equal("Moderator", updatedRole.Name);
        Assert.Equal("Moderator", (await context.Roles.SingleAsync()).Name);
    }

    [Fact]
    public async Task UpdateRoleAsync_AllowCurrentNameIgnoringCase_NoOtherRoleHasName()
    {
        using var context = CreateContext();
        context.Roles.Add(new Role { RoleId = 1, Name = "Admin" });
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var updatedRole = await service.UpdateRoleAsync(
            1,
            new RoleDto { Name = "  aDmIn  " });

        Assert.Equal(1, updatedRole.RoleId);
        Assert.Equal("aDmIn", updatedRole.Name);
        Assert.Single(context.Roles);
    }

    [Fact]
    public async Task DeleteRoleAsync_RemoveRoleAndReturnTrue_RoleExists()
    {
        using var context = CreateContext();
        context.Roles.Add(new Role { RoleId = 1, Name = "Admin" });
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var deleted = await service.DeleteRoleAsync(1);

        Assert.True(deleted);
        Assert.Empty(context.Roles);
    }

    [Fact]
    public async Task DeleteRoleAsync_ThrowNotFoundException_RoleDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteRoleAsync(42));

        Assert.Equal("Role with id 42 was not found.", exception.Message);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static RoleService CreateService(AppDbContext context)
    {
        var configuration = new MapperConfiguration(
            config => config.AddProfile<RoleProfile>(),
            NullLoggerFactory.Instance);
        return new RoleService(context, configuration.CreateMapper());
    }
}