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

public class FavoriteServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static FavoriteService CreateService(AppDbContext context)
    {
        var mapperConfiguration = new MapperConfiguration(configuration =>
            configuration.AddProfile<FavoriteProfile>(),
            NullLoggerFactory.Instance);

        return new FavoriteService(context, mapperConfiguration.CreateMapper());
    }

    private static async Task SeedGamesAsync(AppDbContext context)
    {
        context.Games.AddRange(
            new Game
            {
                Id = 1,
                Name = "First Game",
                MinPlayers = 1,
                MaxPlayers = 4,
                PlayTimeMinutes = 30
            },
            new Game
            {
                Id = 2,
                Name = "Second Game",
                MinPlayers = 2,
                MaxPlayers = 6,
                PlayTimeMinutes = 60
            });

        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(AppDbContext context)
    {
        context.Users.AddRange(
            new User { Id = 10, Email = "user10@example.com", Username = "user10", FullName = "User Ten", PasswordHash = "hash", RoleId = 1 },
            new User { Id = 11, Email = "user11@example.com", Username = "user11", FullName = "User Eleven", PasswordHash = "hash", RoleId = 1 });

        await context.SaveChangesAsync();
    }

    private static async Task SeedFavoritesAsync(AppDbContext context)
    {
        context.Favorites.AddRange(
            new Favorite { Id = 1, UserId = 10, GameId = 1 },
            new Favorite { Id = 2, UserId = 10, GameId = 2 },
            new Favorite { Id = 3, UserId = 11, GameId = 1 });

        await context.SaveChangesAsync();
    }

    private static CreateFavoriteDTO CreateFavoriteDto(int gameId = 1)
    {
        return new CreateFavoriteDTO
        {
            GameId = gameId
        };
    }

    [Fact]
    public async Task GetAllFavoritesAsync_ReturnsAllFavorites()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        await SeedFavoritesAsync(context);
        var service = CreateService(context);

        var result = await service.GetAllFavoritesAsync();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, favorite => favorite.Id == 1 && favorite.UserId == 10 && favorite.GameId == 1);
        Assert.Contains(result, favorite => favorite.Id == 3 && favorite.UserId == 11 && favorite.GameId == 1);
    }

    [Fact]
    public async Task GetFavoritesByUserIdAsync_ReturnsOnlyFavoritesForRequestedUser()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        await SeedFavoritesAsync(context);
        var service = CreateService(context);

        var result = await service.GetFavoritesByUserIdAsync(10);

        Assert.Equal(2, result.Count);
        Assert.All(result, favorite => Assert.Equal(10, favorite.UserId));
        Assert.Contains(result, favorite => favorite.GameId == 1);
        Assert.Contains(result, favorite => favorite.GameId == 2);
    }

    [Fact]
    public async Task GetFavoritesByUserIdAsync_WhenUserHasNoFavoritesReturnsEmptyList()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        var service = CreateService(context);

        var result = await service.GetFavoritesByUserIdAsync(999);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetFavoriteByIdAsync_WhenFavoriteExistsReturnsFavorite()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        await SeedFavoritesAsync(context);
        var service = CreateService(context);

        var result = await service.GetFavoriteByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(10, result.UserId);
        Assert.Equal(1, result.GameId);
    }

    [Fact]
    public async Task GetFavoriteByIdAsync_WhenFavoriteDoesNotExistThrowsNotFound()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetFavoriteByIdAsync(999));

        Assert.Equal("Favorite with id 999 was not found.", exception.Message);
    }

    [Fact]
    public async Task CreateFavoriteAsync_PersistsFavorite()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        await SeedUsersAsync(context);
        var service = CreateService(context);

        var result = await service.CreateFavoriteAsync(CreateFavoriteDto(), 10);

        var savedFavorite = await context.Favorites.SingleAsync();
        Assert.NotEqual(0, result.Id);
        Assert.Equal(10, result.UserId);
        Assert.Equal(1, result.GameId);
        Assert.Equal(result.Id, savedFavorite.Id);
        Assert.Equal(result.UserId, savedFavorite.UserId);
        Assert.Equal(result.GameId, savedFavorite.GameId);
    }

    [Fact]
    public async Task CreateFavoriteAsync_WhenGameDoesNotExistThrowsNotFound()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.CreateFavoriteAsync(CreateFavoriteDto(gameId: 999), 10));

        Assert.Equal("Game with id 999 does not exist.", exception.Message);
    }

    [Fact]
    public async Task CreateFavoriteAsync_WhenFavoriteAlreadyExistsThrowsConflict()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        await SeedUsersAsync(context);
        await SeedFavoritesAsync(context);
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.CreateFavoriteAsync(CreateFavoriteDto(gameId: 1), 10));

        Assert.Equal("This game is already in the user's favorites.", exception.Message);
    }

    [Fact]
    public async Task DeleteFavoriteAsync_WhenFavoriteExistsDeletesIt()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        await SeedUsersAsync(context);
        await SeedFavoritesAsync(context);
        var service = CreateService(context);

        var deleted = await service.DeleteFavoriteAsync(1, 10, "User");

        Assert.True(deleted);
        Assert.Null(await context.Favorites.FindAsync(1));
    }

    [Fact]
    public async Task DeleteFavoriteAsync_WhenFavoriteDoesNotExistThrowsNotFound()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteFavoriteAsync(999, 10, "User"));

        Assert.Equal("Favorite not found.", exception.Message);
    }

    [Fact]
    public async Task DeleteFavoriteAsync_WhenUserDoesNotOwnFavoriteThrowsUnauthorized()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        await SeedUsersAsync(context);
        await SeedFavoritesAsync(context);
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(
            () => service.DeleteFavoriteAsync(1, 11, "User"));

        Assert.Equal("You are not authorized to delete this favorite.", exception.Message);
    }

    [Fact]
    public async Task DeleteFavoriteAsync_WhenAdminDeletesAnotherUsersFavoriteDeletesIt()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        await SeedUsersAsync(context);
        await SeedFavoritesAsync(context);
        var service = CreateService(context);

        var deleted = await service.DeleteFavoriteAsync(1, 11, "Admin");

        Assert.True(deleted);
        Assert.Null(await context.Favorites.FindAsync(1));
    }
}
