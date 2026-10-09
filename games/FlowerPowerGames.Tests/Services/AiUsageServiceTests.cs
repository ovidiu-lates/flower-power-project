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

public class AiUsageServiceTests
{
    [Fact]
    public async Task CreateAiUsageAsync_UsageForExistingUser()
    {
        using var context = CreateContext();
        AddUser(context);
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var createdUsage = await service.CreateAiUsageAsync(CreateDto(750), 1);

        Assert.True(createdUsage.Id > 0);
        Assert.Equal(1, createdUsage.UserId);
        Assert.Equal(0, createdUsage.TotalRequests);
        Assert.Equal(0, createdUsage.TotalPromptUsed);
        var storedUsage = await context.AiUsages.SingleAsync();
        Assert.Equal(createdUsage.Id, storedUsage.Id);
        Assert.Equal(1, storedUsage.UserId);
        Assert.Equal(750, storedUsage.TotalAvailablePrompt);
    }

    [Fact]
    public async Task CreateAiUsageAsync_ThrowsNotFoundException_UserDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.CreateAiUsageAsync(CreateDto(750), 42));

        Assert.Equal("User with id '42' does not exist.", exception.Message);
        Assert.Empty(context.AiUsages);
    }

    [Fact]
    public async Task CreateAiUsageAsync_ThrowsConflictException_UserAlreadyHasUsage()
    {
        using var context = CreateContext();
        AddUser(context);
        context.AiUsages.Add(CreateEntity(1));
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ConflictException>(() =>
            service.CreateAiUsageAsync(CreateDto(750), 1));

        Assert.Equal("AI usage for user with ID 1 already exists.", exception.Message);
        Assert.Single(context.AiUsages);
    }

    [Fact]
    public async Task GetAiUsageByIdAsync_ReturnsMappedUsage_UsageExists()
    {
        using var context = CreateContext();
        AddUser(context);
        context.AiUsages.Add(CreateEntity(1));
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var usage = await service.GetAiUsageByIdAsync(1);

        Assert.Equal(1, usage.Id);
        Assert.Equal(1, usage.UserId);
        Assert.Equal(3, usage.TotalRequests);
        Assert.Equal(125, usage.TotalPromptUsed);
        Assert.Equal(750, usage.TotalAvailablePrompt);
    }

    [Fact]
    public async Task GetAiUsageByIdAsync_ThrowsNotFoundException_UsageDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetAiUsageByIdAsync(42));

        Assert.Equal("AI usage with ID 42 does not exist.", exception.Message);
    }

    [Fact]
    public async Task GetAiUsageByUserIdAsync_ReturnsMappedUsage_UsageExists()
    {
        using var context = CreateContext();
        AddUser(context);
        context.AiUsages.Add(CreateEntity(1));
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var usage = await service.GetAiUsageByUserIdAsync(1);

        Assert.Equal(1, usage.UserId);
        Assert.Equal(3, usage.TotalRequests);
    }

    [Fact]
    public async Task GetAiUsageByUserIdAsync_ThrowsNotFoundException_UsageDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetAiUsageByUserIdAsync(42));

        Assert.Equal("AI usage for user with ID 42 does not exist.", exception.Message);
    }

    [Fact]
    public async Task UpdateAiUsageAsync_UpdatesUsageButKeepsIdAndAvailablePromptLimit()
    {
        using var context = CreateContext();
        AddUser(context);
        context.AiUsages.Add(CreateEntity(1));
        await context.SaveChangesAsync();
        var service = CreateService(context);
        var request = new AiUsageDTO
        {
            Id = 99,
            UserId = 1,
            TotalRequests = 8,
            TotalPromptUsed = 300,
            TotalAvailablePrompt = 1000
        };

        var updatedUsage = await service.UpdateAiUsageAsync(1, request);

        Assert.Equal(1, updatedUsage.Id);
        Assert.Equal(1, updatedUsage.UserId);
        Assert.Equal(8, updatedUsage.TotalRequests);
        Assert.Equal(300, updatedUsage.TotalPromptUsed);
        Assert.Equal(750, updatedUsage.TotalAvailablePrompt);
        var storedUsage = await context.AiUsages.SingleAsync();
        Assert.Equal(1, storedUsage.Id);
        Assert.Equal(750, storedUsage.TotalAvailablePrompt);
    }

    [Fact]
    public async Task UpdateAiUsageAsync_ThrowsNotFoundException_UsageDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.UpdateAiUsageAsync(
    42,
    new AiUsageDTO
    {
        UserId = 1,
        TotalRequests = 0,
        TotalPromptUsed = 0,
        TotalAvailablePrompt = 750
    }));

        Assert.Equal("AI usage with ID 42 does not exist.", exception.Message);
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

    private static void AddUser(AppDbContext context)
    {
        context.Users.Add(new User
        {
            Id = 1,
            Email = "user@example.com",
            Username = "user",
            FullName = "Test User",
            PasswordHash = "password-hash",
            RoleId = 1
        });
    }

    private static AiUsageService CreateService(AppDbContext context)
    {
        var configuration = new MapperConfiguration(
            config => config.AddProfile<AiUsageProfile>(),
            NullLoggerFactory.Instance);
        return new AiUsageService(context, configuration.CreateMapper());
    }

    private static AiUsage CreateEntity(int userId)
    {
        return new AiUsage
        {
            UserId = userId,
            TotalRequests = 3,
            TotalPromptUsed = 125,
            TotalAvailablePrompt = 750
        };
    }

    private static CreateAiUsageDTO CreateDto(int totalAvailablePrompt)
    {
        return new CreateAiUsageDTO
        {
            TotalAvailablePrompt = totalAvailablePrompt
        };
    }

    private static AiUsageDTO CreateAiUsageDto(int userId)
    {
        return new AiUsageDTO
        {
            UserId = userId,
            TotalRequests = 3,
            TotalPromptUsed = 125,
            TotalAvailablePrompt = 750
        };
    }
}
