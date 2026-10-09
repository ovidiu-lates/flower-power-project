using FlowerPowerGames.API.Controllers;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Business.Validators;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FlowerPowerGames.Tests.Controllers;

public class UserPreferenceControllerTests
{
    [Fact]
    public async Task GetUserPreferenceByUserId_ReturnsOkWithPreference()
    {
        var preference = CreatePreference(7);
        var service = new Mock<IUserPreferenceService>();
        service.Setup(item => item.GetUserPreferenceByUserIdAsync(7)).ReturnsAsync(preference);
        var controller = CreateController(service.Object);

        var result = await controller.GetUserPreferenceByUserId();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(preference, okResult.Value);
        service.Verify(item => item.GetUserPreferenceByUserIdAsync(7), Times.Once);
    }

    [Fact]
    public async Task GetUserPreferenceByUserId_PropagatesNotFoundException()
    {
        var service = new Mock<IUserPreferenceService>();
        service.Setup(item => item.GetUserPreferenceByUserIdAsync(42))
            .ThrowsAsync(new NotFoundException("User preference for user with id 42 was not found."));
        var controller = CreateController(service.Object, userId: 42);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            controller.GetUserPreferenceByUserId());

        Assert.Equal("User preference for user with id 42 was not found.", exception.Message);
    }

    [Fact]
    public async Task CreateUserPreference_ReturnsCreatedAtAction()
    {
        var request = CreateRequest();
        var createdPreference = CreatePreference(7);
        createdPreference.Id = 11;
        var service = new Mock<IUserPreferenceService>();
        service.Setup(item => item.CreateUserPreferenceAsync(request, 7)).ReturnsAsync(createdPreference);
        var controller = CreateController(service.Object);

        var result = await controller.CreateUserPreference(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(UserPreferenceController.GetUserPreferenceByUserId), createdResult.ActionName);
        Assert.Equal(7, createdResult.RouteValues!["userId"]);
        Assert.Same(createdPreference, createdResult.Value);
        service.Verify(item => item.CreateUserPreferenceAsync(request, 7), Times.Once);
    }

    [Fact]
    public async Task UpdateUserPreference_ReturnsOkWithUpdatedPreference()
    {
        var request = CreateRequest();
        var updatedPreference = CreatePreference(7);
        updatedPreference.MaxBudget = 150m;
        var service = new Mock<IUserPreferenceService>();
        service.Setup(item => item.UpdateUserPreferenceAsync(7, request)).ReturnsAsync(updatedPreference);
        var controller = CreateController(service.Object);

        var result = await controller.UpdateUserPreference(request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(updatedPreference, okResult.Value);
    }

    private static UserPreferenceController CreateController(IUserPreferenceService service, int userId = 7)
    {
        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.SetupGet(item => item.UserId).Returns(userId
            );
        return new UserPreferenceController(
            service,
            new CreateUserPreferenceDtoValidator(),
            currentUserService.Object);
    }

    private static CreateUserPreferenceDto CreateRequest()
    {
        return new CreateUserPreferenceDto
        {
            MinBudget = 10m,
            MaxBudget = 100m,
            MinPlayTime = 15,
            MaxPlayTime = 120,
            MinPlayers = 1,
            MaxPlayers = 4,
            MinimumAge = 10,
            GenreIds = [1],
            TypeIds = [1]
        };
    }

    private static UserPreferenceDto CreatePreference(int userId)
    {
        return new UserPreferenceDto
        {
            UserId = userId,
            MinBudget = 10m,
            MaxBudget = 100m,
            MinPlayTime = 15,
            MaxPlayTime = 120,
            MinPlayers = 1,
            MaxPlayers = 4,
            MinimumAge = 10,
            GenreIds = [1],
            TypeIds = [1]
        };
    }
}
