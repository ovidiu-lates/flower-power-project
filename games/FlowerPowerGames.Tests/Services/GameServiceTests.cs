using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Mappers;
using FlowerPowerGames.Business.Services;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FlowerPowerGames.UnitTests.Services;

public class GameServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static GameService CreateService(AppDbContext context)
    {
        var mapperConfiguration = new MapperConfiguration(configuration =>
            configuration.AddProfile<GameProfile>(),
            NullLoggerFactory.Instance);

        return new GameService(context, mapperConfiguration.CreateMapper());
    }

    private static async Task SeedLookupsAsync(AppDbContext context)
    {
        context.Genres.AddRange(
            new Genre { Id = 1, Name = "Strategy" },
            new Genre { Id = 2, Name = "Family" });

        context.GameTypes.AddRange(
            new GameType { Id = 1, Name = "Card" },
            new GameType { Id = 2, Name = "Board" });

        await context.SaveChangesAsync();
    }

    private static GameDto CreateGameDto(
        string name = "New Game",
        List<int>? genreIds = null,
        List<int>? typeIds = null)
    {
        return new GameDto
        {
            Name = name,
            Description = "A test game",
            Price = 19.99m,
            MinPlayers = 2,
            MaxPlayers = 4,
            PlayTimeMinutes = 60,
            LearningTimeMinutes = 15,
            MinimumAge = 10,
            ImageUrl = "https://example.com/game.png",
            GenreIds = genreIds ?? [1],
            TypeIds = typeIds ?? [1]
        };
    }

    [Fact]
    public async Task CreateGameAsync_PersistsGameAndRelationships()
    {
        await using var context = CreateContext();
        await SeedLookupsAsync(context);
        var service = CreateService(context);

        var result = await service.CreateGameAsync(
            CreateGameDto(genreIds: [1, 1, 2], typeIds: [1, 1]));

        var savedGame = await context.Games
            .Include(game => game.Genres)
            .Include(game => game.Types)
            .SingleAsync();

        Assert.NotEqual(0, result.Id);
        Assert.Equal("New Game", result.Name);
        Assert.Equal([1, 2], result.GenreIds.OrderBy(id => id));
        Assert.Equal([1], result.TypeIds);
        Assert.Equal([1, 2], savedGame.Genres.Select(genre => genre.Id).OrderBy(id => id));
        Assert.Equal([1], savedGame.Types.Select(type => type.Id));
    }

    [Fact]
    public async Task GetAllGamesAsync_ReturnsGamesWithRelationshipIds()
    {
        await using var context = CreateContext();
        await SeedLookupsAsync(context);
        var service = CreateService(context);
        await service.CreateGameAsync(CreateGameDto());
        await service.CreateGameAsync(CreateGameDto("Second Game", [2], [2]));

        var result = await service.GetAllGamesAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, game => game.Name == "New Game" && game.GenreIds.SequenceEqual([1]));
        Assert.Contains(result, game => game.Name == "Second Game" && game.TypeIds.SequenceEqual([2]));
    }

    [Fact]
    public async Task GetGameByIdAsync_WhenGameDoesNotExist_ReturnsNull()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.GetGameByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateGameAsync_UpdatesScalarsAndRelationships()
    {
        await using var context = CreateContext();
        await SeedLookupsAsync(context);
        var service = CreateService(context);
        var created = await service.CreateGameAsync(CreateGameDto());

        var update = CreateGameDto("Updated Game", [2], [2]);
        update.MaxPlayers = 6;

        var result = await service.UpdateGameAsync(created.Id, update);

        Assert.NotNull(result);
        Assert.Equal("Updated Game", result.Name);
        Assert.Equal(6, result.MaxPlayers);
        Assert.Equal([2], result.GenreIds);
        Assert.Equal([2], result.TypeIds);

        var savedGame = await context.Games
            .Include(game => game.Genres)
            .Include(game => game.Types)
            .SingleAsync();
        Assert.Equal([2], savedGame.Genres.Select(genre => genre.Id));
        Assert.Equal([2], savedGame.Types.Select(type => type.Id));
    }

    [Fact]
    public async Task UpdateGameAsync_WhenGameDoesNotExist_ReturnsNull()
    {
        await using var context = CreateContext();
        await SeedLookupsAsync(context);
        var service = CreateService(context);

        var result = await service.UpdateGameAsync(999, CreateGameDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteGameAsync_ExistingGameDeletesIt()
    {
        await using var context = CreateContext();
        await SeedLookupsAsync(context);
        var service = CreateService(context);
        var created = await service.CreateGameAsync(CreateGameDto());

        var deleted = await service.DeleteGameAsync(created.Id);

        Assert.True(deleted);
        Assert.Empty(await context.Games.ToListAsync());
    }

    [Fact]
    public async Task DeleteGameAsync_WhenGameDoesNotExistReturnsFalse()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var deleted = await service.DeleteGameAsync(999);

        Assert.False(deleted);
    }

    [Fact]
    public async Task CreateGameAsync_CleansName()
    {
        await using var context = CreateContext();
        await SeedLookupsAsync(context);
        var service = CreateService(context);

        var result = await service.CreateGameAsync(CreateGameDto("  New Game  "));

        Assert.Equal("New Game", result.Name);
    }

    [Fact]
    public async Task CreateGameAsync_WhenNameAlreadyExistsThrowsConflict()
    {
        await using var context = CreateContext();
        await SeedLookupsAsync(context);
        var service = CreateService(context);
        await service.CreateGameAsync(CreateGameDto("Existing Game"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateGameAsync(CreateGameDto(" existing game ")));

        Assert.Contains("existing game", exception.Message);
    }

    [Fact]
    public async Task CreateGameAsync_WhenPlayerRangeIsInvalidThrowsArgumentException()
    {
        await using var context = CreateContext();
        await SeedLookupsAsync(context);
        var service = CreateService(context);
        var dto = CreateGameDto();
        dto.MinPlayers = 5;
        dto.MaxPlayers = 4;

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateGameAsync(dto));

        Assert.Contains("Maximum number of players", exception.Message);
    }

    [Fact]
    public async Task CreateGameAsync_WhenLookupIdDoesNotExistThrowsArgumentException()
    {
        await using var context = CreateContext();
        await SeedLookupsAsync(context);
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateGameAsync(CreateGameDto(genreIds: [999])));

        Assert.Contains("999", exception.Message);
    }

    [Fact]
    public async Task CreateGameAsync_WhenRelationshipIdsAreNotProvidedThrowsArgumentException()
    {
        await using var context = CreateContext();
        await SeedLookupsAsync(context);
        var service = CreateService(context);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateGameAsync(CreateGameDto(genreIds: [], typeIds: [])));
    }
}
