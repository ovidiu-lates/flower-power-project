using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Services;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace FlowerPowerGames.UnitTests.Services;

public class RatingServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString())
                .Options;

        return new AppDbContext(options);
    }

    private static Mock<IMapper> CreateMapper()
    {
        var mapper = new Mock<IMapper>();

        mapper
            .Setup(x =>
                x.Map<Rating>(
                    It.IsAny<CreateRatingDTO>()))
            .Returns((CreateRatingDTO dto) =>
                new Rating
                {
                    GameId = dto.GameId,
                    Score = dto.Score,
                    Review = dto.Review
                });

        mapper
            .Setup(x =>
                x.Map<RatingDto>(
                    It.IsAny<Rating>()))
            .Returns((Rating rating) =>
                new RatingDto
                {
                    Id = rating.Id,
                    UserId = rating.UserId,
                    GameId = rating.GameId,
                    Score = rating.Score,
                    Review = rating.Review,
                    CreatedAt = rating.CreatedAt,
                    UpdatedAt = rating.UpdatedAt
                });

        mapper
            .Setup(x =>
                x.Map<List<RatingDto>>(
                    It.IsAny<object>()))
            .Returns((object source) =>
            {
                var ratings =
                    (IEnumerable<Rating>)source;

                return ratings
                    .Select(rating => new RatingDto
                    {
                        Id = rating.Id,
                        UserId = rating.UserId,
                        GameId = rating.GameId,
                        Score = rating.Score,
                        Review = rating.Review
                    })
                    .ToList();
            });

        return mapper;
    }

    [Fact]
    public async Task CreateRating_WithExistingGameAndUser_SavesRating()
    {
        await using var context =
            CreateContext();

        context.Games.Add(new Game
        {
            Id = 5,
            Name = "Test Game"
        });

        context.Users.Add(new User
        {
            Id = 10,
            Email = "user@test.com",
            Username = "testuser",
            FullName = "Test User",
            PasswordHash = "hash",
            RoleId = 1
        });

        await context.SaveChangesAsync();

        var service = new RatingService(
            context,
            CreateMapper().Object);

        var request = new CreateRatingDTO
        {
            GameId = 5,
            Score = 8,
            Review = "Good game"
        };

        var result =
            await service.CreateRatingAsync(
                request,
                10);

        Assert.Equal(5, result.GameId);
        Assert.Equal(10, result.UserId);
        Assert.Equal(8, result.Score);

        var savedRating =
            await context.Ratings.SingleAsync();

        Assert.Equal(10, savedRating.UserId);
        Assert.Equal(5, savedRating.GameId);
    }

    [Fact]
    public async Task CreateRating_WhenGameDoesNotExist_ThrowsNotFound()
    {
        await using var context =
            CreateContext();

        context.Users.Add(new User
        {
            Id = 10,
            Email = "user@test.com",
            Username = "testuser",
            FullName = "Test User",
            PasswordHash = "hash",
            RoleId = 1
        });

        await context.SaveChangesAsync();

        var service = new RatingService(
            context,
            CreateMapper().Object);

        var request = new CreateRatingDTO
        {
            GameId = 999,
            Score = 8
        };

        await Assert.ThrowsAsync<NotFoundException>(
            () =>
                service.CreateRatingAsync(
                    request,
                    10));
    }

    [Fact]
    public async Task CreateRating_WhenUserDoesNotExist_ThrowsNotFound()
    {
        await using var context =
            CreateContext();

        context.Games.Add(new Game
        {
            Id = 5,
            Name = "Test Game"
        });

        await context.SaveChangesAsync();

        var service = new RatingService(
            context,
            CreateMapper().Object);

        var request = new CreateRatingDTO
        {
            GameId = 5,
            Score = 8
        };

        await Assert.ThrowsAsync<NotFoundException>(
            () =>
                service.CreateRatingAsync(
                    request,
                    999));
    }

    [Fact]
    public async Task GetRatingById_WhenMissing_ThrowsNotFound()
    {
        await using var context =
            CreateContext();

        var service = new RatingService(
            context,
            CreateMapper().Object);

        await Assert.ThrowsAsync<NotFoundException>(
            () =>
                service.GetRatingByIdAsync(999));
    }

    [Fact]
    public async Task UpdateRating_UpdatesScoreAndReview()
    {
        await using var context =
            CreateContext();

        context.Ratings.Add(new Rating
        {
            Id = 1,
            UserId = 10,
            GameId = 5,
            Score = 5,
            Review = "Old review"
        });

        await context.SaveChangesAsync();

        var service = new RatingService(
            context,
            CreateMapper().Object);

        var request = new RatingDto
        {
            Score = 9,
            Review = "Updated review"
        };

        var result =
            await service.UpdateRatingAsync(
                1,
                request);

        Assert.Equal(9, result.Score);
        Assert.Equal(
            "Updated review",
            result.Review);

        var savedRating =
            await context.Ratings.FindAsync(1);

        Assert.NotNull(savedRating);
        Assert.Equal(9, savedRating.Score);
    }

    [Fact]
    public async Task UpdateRating_WhenMissing_ThrowsNotFound()
    {
        await using var context =
            CreateContext();

        var service = new RatingService(
            context,
            CreateMapper().Object);

        await Assert.ThrowsAsync<NotFoundException>(
            () =>
                service.UpdateRatingAsync(
                    999,
                    new RatingDto
                    {
                        Score = 8
                    }));
    }

    [Fact]
    public async Task DeleteRating_DeletesExistingRating()
    {
        await using var context =
            CreateContext();

        context.Ratings.Add(new Rating
        {
            Id = 1,
            UserId = 10,
            GameId = 5,
            Score = 8
        });

        await context.SaveChangesAsync();

        var service = new RatingService(
            context,
            CreateMapper().Object);

        var result =
            await service.DeleteRatingAsync(1);

        Assert.True(result);
        Assert.Empty(context.Ratings);
    }

    [Fact]
    public async Task DeleteRating_WhenMissing_ThrowsNotFound()
    {
        await using var context =
            CreateContext();

        var service = new RatingService(
            context,
            CreateMapper().Object);

        await Assert.ThrowsAsync<NotFoundException>(
            () =>
                service.DeleteRatingAsync(999));
    }
}