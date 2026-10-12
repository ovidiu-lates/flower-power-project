using FlowerPowerGames.Business.AI;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Business.Services;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace FlowerPowerGames.Tests.Services;

public class RecommendationServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetRecommendations_WithNoCandidateGames_ReturnsEmptyResponse()
    {
        await using var context = CreateContext();

        var aiClient =
            new Mock<IAiRecommendationClient>();

        var usageService =
            new Mock<IAiUsageService>();

        usageService
            .Setup(x => x.EnsureAiUsageAvailableAsync(
                1,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new RecommendationService(
            context,
            aiClient.Object,
            usageService.Object);

        var request = new RecommendationRequestDto
        {
            Message = "Recommend a board game."
        };

        var result = await service.GetRecommendationsAsync(
            1,
            request,
            cancellationToken: CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Recommendations);

        aiClient.Verify(
            x => x.GetRecommendationsAsync(
                It.IsAny<RecommendationContext>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        usageService.Verify(
            x => x.RecordAiRequestAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetRecommendations_WithCandidateGame_ReturnsAiRecommendation()
    {
        await using var context = CreateContext();

        context.Games.Add(new Game
        {
            Id = 1,
            Name = "Catan",
            Description = "A strategy board game.",
            Price = 40,
            MinPlayers = 3,
            MaxPlayers = 4,
            PlayTimeMinutes = 90,
            LearningTimeMinutes = 15,
            MinimumAge = 10,
            Rating = 4.5m
        });

        await context.SaveChangesAsync();

        var aiClient =
            new Mock<IAiRecommendationClient>();

        var usageService =
            new Mock<IAiUsageService>();

        usageService
            .Setup(x => x.EnsureAiUsageAvailableAsync(
                1,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        aiClient
            .Setup(x => x.GetRecommendationsAsync(
                It.IsAny<RecommendationContext>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AiRecommendationResult
            {
                Recommendations =
                [
                    new AiRecommendedGame
                    {
                        GameId = 1,
                        Reason = "It matches your strategy preference."
                    }
                ],
                Usage = new AiTokenUsage
                {
                    InputTokens = 100,
                    OutputTokens = 20,
                    TotalTokens = 120
                }
            });

        var service = new RecommendationService(
            context,
            aiClient.Object,
            usageService.Object);

        var request = new RecommendationRequestDto
        {
            Message = "Recommend a strategy game."
        };

        var result = await service.GetRecommendationsAsync(
            1,
            request,
            cancellationToken: CancellationToken.None);

        var recommendation =
            Assert.Single(result.Recommendations);

        Assert.Equal(1, recommendation.GameId);
        Assert.Equal(
            "Catan",
            recommendation.Name);
        Assert.Equal(
            "It matches your strategy preference.",
            recommendation.Reason);

        usageService.Verify(
            x => x.RecordAiRequestAsync(
                1,
                120,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetRecommendations_WithInvalidCount_ThrowsArgumentException()
    {
        await using var context = CreateContext();

        var service = new RecommendationService(
            context,
            Mock.Of<IAiRecommendationClient>(),
            Mock.Of<IAiUsageService>());

        var request = new RecommendationRequestDto
        {
            Message = "Recommend a game."
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetRecommendationsAsync(
                1,
                request,
                recommendationCount: 0,
                cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task GetRecommendations_RecordsTokenUsage()
    {
        await using var context = CreateContext();

        context.Games.Add(new Game
        {
            Id = 1,
            Name = "Catan",
            Price = 40,
            MinPlayers = 3,
            MaxPlayers = 4,
            PlayTimeMinutes = 90,
            LearningTimeMinutes = 15,
            MinimumAge = 10,
            Rating = 4.5m
        });

        await context.SaveChangesAsync();

        var usageService =
            new Mock<IAiUsageService>();

        usageService
            .Setup(x => x.EnsureAiUsageAvailableAsync(
                1,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var aiClient =
            new Mock<IAiRecommendationClient>();

        aiClient
            .Setup(x => x.GetRecommendationsAsync(
                It.IsAny<RecommendationContext>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AiRecommendationResult
            {
                Recommendations = [],
                Usage = new AiTokenUsage
                {
                    TotalTokens = 75
                }
            });

        var service = new RecommendationService(
            context,
            aiClient.Object,
            usageService.Object);

        await service.GetRecommendationsAsync(
            1,
            new RecommendationRequestDto
            {
                Message = "Recommend a game."
            },
            cancellationToken: CancellationToken.None);

        usageService.Verify(
            x => x.RecordAiRequestAsync(
                1,
                75,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}