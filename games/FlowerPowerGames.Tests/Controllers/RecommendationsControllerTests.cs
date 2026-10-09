using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FlowerPowerGames.UnitTests.Controllers;

public class RecommendationsControllerTests
{
    [Fact]
    public async Task GetRecommendations_WithAuthenticatedUser_ReturnsOk()
    {
        var recommendationService =
            new Mock<IRecommendationService>();

        var currentUserService =
            new Mock<ICurrentUserService>();

        currentUserService
            .Setup(x => x.UserId)
            .Returns(1);

        var response = new RecommendationResponseDto
        {
            Recommendations =
            [
                new RecommendedGameDto
                {
                    GameId = 10,
                    Name = "Catan",
                    Reason = "Matches your preferences."
                }
            ]
        };

        recommendationService
            .Setup(x => x.GetRecommendationsAsync(
                1,
                It.IsAny<RecommendationRequestDto>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new RecommendationsController(
            recommendationService.Object,
            currentUserService.Object);

        var request = new RecommendationRequestDto
        {
            Message = "Recommend a strategy game."
        };

        var result = await controller.GetRecommendations(
            request,
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        Assert.Same(response, okResult.Value);

        recommendationService.Verify(
            x => x.GetRecommendationsAsync(
                1,
                request,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}