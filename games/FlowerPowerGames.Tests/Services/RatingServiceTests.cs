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

public class RatingServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static RatingService CreateService(AppDbContext context)
    {
        var mapperConfiguration = new MapperConfiguration(configuration =>
            configuration.AddProfile<RatingProfile>(),
            NullLoggerFactory.Instance);

        return new RatingService(context, mapperConfiguration.CreateMapper());
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

    private static async Task SeedRatingsAsync(AppDbContext context)
    {
        context.Ratings.AddRange(
            new Rating
            {
                Id = 1,
                UserId = 10,
                GameId = 1,
                Score = 5,
                Review = "Excellent",
                CreatedAt = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc)
            },
            new Rating
            {
                Id = 2,
                UserId = 11,
                GameId = 1,
                Score = 4,
                Review = "Very good",
                CreatedAt = new DateTime(2024, 1, 2, 10, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 2, 10, 0, 0, DateTimeKind.Utc)
            },
            new Rating
            {
                Id = 3,
                UserId = 12,
                GameId = 2,
                Score = 3,
                Review = "Good",
                CreatedAt = new DateTime(2024, 1, 3, 10, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 3, 10, 0, 0, DateTimeKind.Utc)
            });

        await context.SaveChangesAsync();
    }

    private static RatingDto CreateRatingDto(
        int gameId = 1,
        int score = 5,
        string? review = "Excellent")
    {
        return new RatingDto
        {
            UserId = 10,
            GameId = gameId,
            Score = score,
            Review = review
        };
    }

    [Fact]
    public async Task GetAllRatingsAsync_ReturnsAllRatings()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        await SeedRatingsAsync(context);
        var service = CreateService(context);

        var result = await service.GetAllRatingsAsync();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, rating => rating.Id == 1 && rating.GameId == 1);
        Assert.Contains(result, rating => rating.Id == 3 && rating.GameId == 2);
    }

    [Fact]
    public async Task GetRatingsByGameIdAsync_ReturnsOnlyRatingsForRequestedGame()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        await SeedRatingsAsync(context);
        var service = CreateService(context);

        var result = await service.GetRatingsByGameIdAsync(1);

        Assert.Equal(2, result.Count);
        Assert.All(result, rating => Assert.Equal(1, rating.GameId));
    }

    [Fact]
    public async Task GetRatingsByGameIdAsync_WhenGameHasNoRatingsReturnsEmptyList()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        var service = CreateService(context);

        var result = await service.GetRatingsByGameIdAsync(1);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRatingByIdAsync_WhenRatingExistsReturnsRating()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        await SeedRatingsAsync(context);
        var service = CreateService(context);

        var result = await service.GetRatingByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(10, result.UserId);
        Assert.Equal(1, result.GameId);
        Assert.Equal(5, result.Score);
        Assert.Equal("Excellent", result.Review);
    }

    [Fact]
    public async Task GetRatingByIdAsync_WhenRatingDoesNotExistReturnsNull()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.GetRatingByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateRatingAsync_PersistsRatingAndSetsTimestamps()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        var service = CreateService(context);

        var before = DateTime.UtcNow;
        var result = await service.CreateRatingAsync(CreateRatingDto());
        var after = DateTime.UtcNow;

        var savedRating = await context.Ratings.SingleAsync();
        Assert.NotEqual(0, result.Id);
        Assert.Equal(1, result.GameId);
        Assert.Equal(5, result.Score);
        Assert.Equal("Excellent", result.Review);
        Assert.InRange(result.CreatedAt, before, after);
        Assert.Equal(result.CreatedAt, result.UpdatedAt);
        Assert.Equal(result.CreatedAt, savedRating.CreatedAt);
        Assert.Equal(result.UpdatedAt, savedRating.UpdatedAt);
        Assert.Equal("Excellent", savedRating.Review);
    }

    [Fact]
    public async Task CreateRatingAsync_WhenGameDoesNotExistThrowsArgumentException()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateRatingAsync(CreateRatingDto(gameId: 999)));

        Assert.Equal("Game does not exist", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task CreateRatingAsync_WhenScoreIsOutsideRangeThrowsArgumentException(int score)
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateRatingAsync(CreateRatingDto(score: score)));

        Assert.Equal("Score should be between 1-5 to submit the rating", exception.Message);
    }

    [Fact]
    public async Task UpdateRatingAsync_UpdatesScoreReviewAndTimestamp()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        await SeedRatingsAsync(context);
        var original = await context.Ratings.FindAsync(1);
        var originalCreatedAt = original!.CreatedAt;
        var service = CreateService(context);
        var update = CreateRatingDto(gameId: 2, score: 2, review: "Changed");
        update.UserId = 99;

        var before = DateTime.UtcNow;
        var result = await service.UpdateRatingAsync(1, update);
        var after = DateTime.UtcNow;

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(10, result.UserId);
        Assert.Equal(1, result.GameId);
        Assert.Equal(2, result.Score);
        Assert.Equal("Changed", result.Review);
        Assert.Equal(originalCreatedAt, result.CreatedAt);
        Assert.InRange(result.UpdatedAt, before, after);

        var savedRating = await context.Ratings.FindAsync(1);
        Assert.Equal(10, savedRating!.UserId);
        Assert.Equal(1, savedRating.GameId);
        Assert.Equal(2, savedRating!.Score);
        Assert.Equal("Changed", savedRating.Review);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task UpdateRatingAsync_WhenScoreIsOutsideRangeThrowsArgumentException(int score)
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        await SeedRatingsAsync(context);
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateRatingAsync(1, CreateRatingDto(score: score)));

        Assert.Equal("Score should be between 1-5 to submit the rating", exception.Message);
    }

    [Fact]
    public async Task UpdateRatingAsync_WhenRatingDoesNotExistReturnsNull()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.UpdateRatingAsync(999, CreateRatingDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteRatingAsync_WhenRatingExistsDeletesIt()
    {
        await using var context = CreateContext();
        await SeedGamesAsync(context);
        await SeedRatingsAsync(context);
        var service = CreateService(context);

        var deleted = await service.DeleteRatingAsync(1);

        Assert.True(deleted);
        Assert.Null(await context.Ratings.FindAsync(1));
    }

    [Fact]
    public async Task DeleteRatingAsync_WhenRatingDoesNotExistReturnsFalse()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var deleted = await service.DeleteRatingAsync(999);

        Assert.False(deleted);
    }
}
