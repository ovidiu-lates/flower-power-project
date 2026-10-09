using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FlowerPowerGames.API.Controllers;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;

using FluentValidation;
using FluentValidation.Results;

namespace FlowerPowerGames.Tests.Controllers;

public class GamesControllerTests
{
    [Fact]
    public async Task GetAllGames_ReturnsOkWithList()
    {
        // Arrange
        var mockService = new Mock<IGameService>();
        var mockValidator = new Mock<IValidator<GameDto>>();

        var games = new List<GameDto> { new GameDto { Id = 1, Name = "Test Game" } };
        mockService.Setup(s => s.GetAllGamesAsync()).ReturnsAsync(games); 
        
        var controller = new GamesController(mockService.Object, mockValidator.Object);

        // Act
        var result = await controller.GetAllGames();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<GameDto>>(ok.Value);
        Assert.Single(returned);
    }

    [Fact]
    public async Task GetGameById_WhenServiceThrowsNotFoundExceptionPropagatesException()
    {
        var mockService = new Mock<IGameService>();
        var mockValidator = new Mock<IValidator<GameDto>>();

        mockService.Setup(s => s.GetGameByIdAsync(42))
            .ThrowsAsync(new NotFoundException("Game with id 42 was not found."));
        var controller = new GamesController(mockService.Object, mockValidator.Object);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => controller.GetGameById(42));

        Assert.Equal("Game with id 42 was not found.", exception.Message);
    }

    [Fact]
    public async Task CreateGame_WhenRequestIsInvalidReturnsBadRequest()
    {
        var mockService = new Mock<IGameService>();
        var mockValidator = new Mock<IValidator<GameDto>>();
        mockValidator.Setup(v => v.ValidateAsync(
                It.IsAny<GameDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([
                new ValidationFailure("Name", "Name is required.")]));

        var controller = new GamesController(mockService.Object, mockValidator.Object);

        var dto = new GameDto { Name = "", GenreIds = new List<int> { 1 }, TypeIds = new List<int> { 1 } };

        var result = await controller.CreateGame(dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(bad.Value);
        mockService.Verify(
            service => service.CreateGameAsync(It.IsAny<GameDto>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateGame_ReturnsCreatedAtAction()
    {
        var mockService = new Mock<IGameService>();
        var mockValidator = new Mock<IValidator<GameDto>>();
        mockValidator.Setup(v => v.ValidateAsync(
                It.IsAny<GameDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var created = new GameDto { Id = 7, Name = "Created", GenreIds = new List<int> { 1 }, TypeIds = new List<int> { 1 } };
        mockService.Setup(s => s.CreateGameAsync(It.IsAny<GameDto>())).ReturnsAsync(created);
        var controller = new GamesController(mockService.Object, mockValidator.Object);

        var dto = new GameDto { Name = "Created", GenreIds = new List<int> { 1 }, TypeIds = new List<int> { 1 } };

        var result = await controller.CreateGame(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(GamesController.GetGameById), createdResult.ActionName);
        var returnedGame = Assert.IsType<GameDto>(createdResult.Value);
        Assert.Equal(created.Id, returnedGame.Id);
    }
}