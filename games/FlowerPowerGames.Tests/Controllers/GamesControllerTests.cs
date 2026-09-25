using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FlowerPowerGames.API.Controllers;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Tests.Controllers;

public class GamesControllerTests
{
    [Fact]
    public async Task GetAllGames_ReturnsOkWithList()
    {
        // Arrange
        var mockService = new Mock<IGameService>();
        var games = new List<GameDto> { new GameDto { Id = 1, Name = "Test Game" } };
        mockService.Setup(s => s.GetAllGamesAsync()).ReturnsAsync(games);
        var controller = new GamesController(mockService.Object);

        // Act
        var result = await controller.GetAllGames();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<GameDto>>(ok.Value);
        Assert.Single(returned);
    }

    [Fact]
    public async Task GetGameById_NotFound_ReturnsNotFound()
    {
        var mockService = new Mock<IGameService>();
        mockService.Setup(s => s.GetGameByIdAsync(42)).ReturnsAsync((GameDto?)null);
        var controller = new GamesController(mockService.Object);

        var result = await controller.GetGameById(42);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateGame_ServiceThrowsArgumentException_ReturnsBadRequest()
    {
        var mockService = new Mock<IGameService>();
        mockService.Setup(s => s.CreateGameAsync(It.IsAny<GameDto>()))
            .ThrowsAsync(new System.ArgumentException("invalid"));
        var controller = new GamesController(mockService.Object);

        var dto = new GameDto { Name = "x", GenreIds = new List<int> { 1 }, TypeIds = new List<int> { 1 } };

        var result = await controller.CreateGame(dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("invalid", bad.Value);
    }

    [Fact]
    public async Task CreateGame_ReturnsCreatedAtAction()
    {
        var mockService = new Mock<IGameService>();
        var created = new GameDto { Id = 7, Name = "Created", GenreIds = new List<int> { 1 }, TypeIds = new List<int> { 1 } };
        mockService.Setup(s => s.CreateGameAsync(It.IsAny<GameDto>())).ReturnsAsync(created);
        var controller = new GamesController(mockService.Object);

        var dto = new GameDto { Name = "Created", GenreIds = new List<int> { 1 }, TypeIds = new List<int> { 1 } };

        var result = await controller.CreateGame(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(GamesController.GetGameById), createdResult.ActionName);
        Assert.Equal(created.Id, ((GameDto)createdResult.Value).Id);
    }
}