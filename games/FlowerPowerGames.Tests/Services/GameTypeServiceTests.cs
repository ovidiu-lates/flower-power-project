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

public class GameTypeServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static GameTypeService CreateService(AppDbContext context)
    {
        var mapperConfiguration = new MapperConfiguration(configuration =>
            configuration.AddProfile<TypeProfile>(),
            NullLoggerFactory.Instance);

        return new GameTypeService(context, mapperConfiguration.CreateMapper());
    }

    private static async Task SeedTypesAsync(AppDbContext context)
    {
        context.GameTypes.AddRange(
            new GameType { Id = 1, Name = "Card" },
            new GameType { Id = 2, Name = "Board" });

        await context.SaveChangesAsync();
    }

    private static GameTypeDto CreateGameTypeDto(string name = "Dice")
    {
        return new GameTypeDto { Name = name };
    }

    [Fact]
    public async Task GetAllGameTypesAsync_ReturnsTypesOrderedByName()
    {
        await using var context = CreateContext();
        await SeedTypesAsync(context);
        var service = CreateService(context);

        var result = await service.GetAllGameTypesAsync();

        Assert.Equal(["Board", "Card"], result.Select(type => type.Name));
    }

    [Fact]
    public async Task GetGameTypeByIdAsync_WhenTypeDoesNotExistThrowsNotFound()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetGameTypeByIdAsync(999));

        Assert.Equal("Game type with id 999 was not found.", exception.Message);
    }

    [Fact]
    public async Task CreateGameTypeAsync_PersistsTrimmedName()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.CreateGameTypeAsync(CreateGameTypeDto("  Dice  "));

        Assert.NotEqual(0, result.Id);
        Assert.Equal("Dice", result.Name);
        Assert.Equal("Dice", (await context.GameTypes.SingleAsync()).Name);
    }

    [Fact]
    public async Task CreateGameTypeAsync_WhenNameAlreadyExistsThrowsConflict()
    {
        await using var context = CreateContext();
        await SeedTypesAsync(context);
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.CreateGameTypeAsync(CreateGameTypeDto(" card ")));

        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public async Task CreateGameTypeAsync_WhenNameIsBlankThrowsArgumentException()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateGameTypeAsync(CreateGameTypeDto("   ")));

        Assert.Equal("Type name cannot be empty.", exception.Message);
    }

    [Fact]
    public async Task UpdateGameTypeAsync_UpdatesName()
    {
        await using var context = CreateContext();
        await SeedTypesAsync(context);
        var service = CreateService(context);

        var result = await service.UpdateGameTypeAsync(1, CreateGameTypeDto("  Dice  "));

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Dice", result.Name);
        Assert.Equal("Dice", (await context.GameTypes.FindAsync(1))!.Name);
    }

    [Fact]
    public async Task UpdateGameTypeAsync_WhenTypeDoesNotExistThrowsNotFound()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateGameTypeAsync(999, CreateGameTypeDto()));

        Assert.Equal("Game type with id 999 was not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateGameTypeAsync_WhenNameBelongsToAnotherTypeThrowsConflict()
    {
        await using var context = CreateContext();
        await SeedTypesAsync(context);
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.UpdateGameTypeAsync(1, CreateGameTypeDto(" board ")));

        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public async Task DeleteGameTypeAsync_WhenTypeIsUnusedDeletesIt()
    {
        await using var context = CreateContext();
        await SeedTypesAsync(context);
        var service = CreateService(context);

        var deleted = await service.DeleteGameTypeAsync(1);

        Assert.True(deleted);
        Assert.Null(await context.GameTypes.FindAsync(1));
    }

    [Fact]
    public async Task DeleteGameTypeAsync_WhenTypeDoesNotExistThrowsNotFound()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteGameTypeAsync(999));

        Assert.Equal("Game type with id 999 was not found.", exception.Message);
    }

    [Fact]
    public async Task DeleteGameTypeAsync_WhenTypeIsAssignedToGameThrowsConflict()
    {
        await using var context = CreateContext();
        await SeedTypesAsync(context);
        var type = await context.GameTypes.FindAsync(1);
        context.Games.Add(new Game
        {
            Name = "Test Game",
            MinPlayers = 1,
            MaxPlayers = 2,
            PlayTimeMinutes = 30,
            Types = [type!]
        });
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.DeleteGameTypeAsync(1));

        Assert.Contains("assigned to one or more games", exception.Message);
        Assert.NotNull(await context.GameTypes.FindAsync(1));
    }
}
