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

public class GenreServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static GenreService CreateService(AppDbContext context)
    {
        var mapperConfiguration = new MapperConfiguration(configuration =>
            configuration.AddProfile<GenreProfile>(),
            NullLoggerFactory.Instance);

        return new GenreService(context, mapperConfiguration.CreateMapper());
    }

    private static async Task SeedGenresAsync(AppDbContext context)
    {
        context.Genres.AddRange(
            new Genre { Id = 1, Name = "Strategy" },
            new Genre { Id = 2, Name = "Family" });

        await context.SaveChangesAsync();
    }

    private static GenreDto CreateGenreDto(string name = "Party")
    {
        return new GenreDto { Name = name };
    }

    [Fact]
    public async Task GetAllGenresAsync_ReturnsGenresOrderedByName()
    {
        await using var context = CreateContext();
        await SeedGenresAsync(context);
        var service = CreateService(context);

        var result = await service.GetAllGenresAsync();

        Assert.Equal(["Family", "Strategy"], result.Select(genre => genre.Name));
    }

    [Fact]
    public async Task GetGenreByIdAsync_WhenGenreDoesNotExistReturnsNull()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.GetGenreByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetGenreByIdAsync_WhenGenreExistsReturnsGenre()
    {
        await using var context = CreateContext();
        await SeedGenresAsync(context);
        var service = CreateService(context);

        var result = await service.GetGenreByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Strategy", result.Name);
    }

    [Fact]
    public async Task CreateGenreAsync_PersistsTrimmedName()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.CreateGenreAsync(CreateGenreDto("  Party  "));

        Assert.NotEqual(0, result.Id);
        Assert.Equal("Party", result.Name);
        Assert.Equal("Party", (await context.Genres.SingleAsync()).Name);
    }

    [Fact]
    public async Task CreateGenreAsync_WhenNameAlreadyExistsThrowsConflict()
    {
        await using var context = CreateContext();
        await SeedGenresAsync(context);
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateGenreAsync(CreateGenreDto(" strategy ")));

        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public async Task CreateGenreAsync_WhenNameIsBlankThrowsArgumentException()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateGenreAsync(CreateGenreDto("   ")));

        Assert.Equal("Genre name cannot be empty.", exception.Message);
    }

    [Fact]
    public async Task UpdateGenreAsync_UpdatesName()
    {
        await using var context = CreateContext();
        await SeedGenresAsync(context);
        var service = CreateService(context);

        var result = await service.UpdateGenreAsync(1, CreateGenreDto("  Party  "));

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Party", result.Name);
        Assert.Equal("Party", (await context.Genres.FindAsync(1))!.Name);
    }

    [Fact]
    public async Task UpdateGenreAsync_WhenGenreDoesNotExistReturnsNull()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.UpdateGenreAsync(999, CreateGenreDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateGenreAsync_WhenNameBelongsToAnotherGenreThrowsConflict()
    {
        await using var context = CreateContext();
        await SeedGenresAsync(context);
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.UpdateGenreAsync(1, CreateGenreDto(" family ")));

        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public async Task UpdateGenreAsync_WhenNameBelongsToCurrentGenreUpdatesSuccessfully()
    {
        await using var context = CreateContext();
        await SeedGenresAsync(context);
        var service = CreateService(context);

        var result = await service.UpdateGenreAsync(1, CreateGenreDto("  strategy  "));

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("strategy", result.Name);
        Assert.Equal("strategy", (await context.Genres.FindAsync(1))!.Name);
    }

    [Fact]
    public async Task DeleteGenreAsync_WhenGenreIsUnusedDeletesIt()
    {
        await using var context = CreateContext();
        await SeedGenresAsync(context);
        var service = CreateService(context);

        var deleted = await service.DeleteGenreAsync(1);

        Assert.True(deleted);
        Assert.Null(await context.Genres.FindAsync(1));
    }

    [Fact]
    public async Task DeleteGenreAsync_WhenGenreDoesNotExistReturnsFalse()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var deleted = await service.DeleteGenreAsync(999);

        Assert.False(deleted);
    }

    [Fact]
    public async Task DeleteGenreAsync_WhenGenreIsAssignedToGameThrowsConflict()
    {
        await using var context = CreateContext();
        await SeedGenresAsync(context);
        var genre = await context.Genres.FindAsync(1);
        context.Games.Add(new Game
        {
            Name = "Test Game",
            MinPlayers = 1,
            MaxPlayers = 2,
            PlayTimeMinutes = 30,
            Genres = [genre!]
        });
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.DeleteGenreAsync(1));

        Assert.Contains("assigned to one or more games", exception.Message);
        Assert.NotNull(await context.Genres.FindAsync(1));
    }
}
