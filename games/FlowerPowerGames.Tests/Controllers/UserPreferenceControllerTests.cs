using FluentValidation;
using FluentValidation.Results;
using FlowerPowerGames.API.Controllers;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
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

        var result = await controller.GetUserPreferenceByUserId(7);

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
        var controller = CreateController(service.Object);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            controller.GetUserPreferenceByUserId(42));

        Assert.Equal("User preference for user with id 42 was not found.", exception.Message);
    }

    [Fact]
    public async Task CreateUserPreference_CreatedAtAction_ValidationSucceeds()
    {
        var request = CreatePreference(7);
        var createdPreference = CreatePreference(7);
        createdPreference.Id = 11;
        var service = new Mock<IUserPreferenceService>();
        service.Setup(item => item.CreateUserPreferenceAsync(request)).ReturnsAsync(createdPreference);
        var controller = CreateController(service.Object);

        var result = await controller.CreateUserPreference(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(UserPreferenceController.GetUserPreferenceByUserId), createdResult.ActionName);
        Assert.Equal(7, createdResult.RouteValues!["userId"]);
        Assert.Same(createdPreference, createdResult.Value);
    }

    [Fact]
    public async Task CreateUserPreference_ReturnsBadRequest_ValidationFails()
    {
        var request = CreatePreference(0);
        var service = new Mock<IUserPreferenceService>();
        var validator = new Mock<IValidator<UserPreferenceDTO>>();
        validator.Setup(item => item.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure(nameof(UserPreferenceDTO.UserId), "UserId must be a positive integer.")
            }));
        var controller = CreateController(service.Object, validator.Object);

        var result = await controller.CreateUserPreference(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Single(Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequest.Value));
        service.Verify(item => item.CreateUserPreferenceAsync(It.IsAny<UserPreferenceDTO>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserPreference_ReturnsOkWithUpdatedPreference()
    {
        var request = CreatePreference(7);
        var updatedPreference = CreatePreference(7);
        updatedPreference.MaxBudget = 150m;
        var service = new Mock<IUserPreferenceService>();
        service.Setup(item => item.UpdateUserPreferenceAsync(7, request)).ReturnsAsync(updatedPreference);
        var controller = CreateController(service.Object);

        var result = await controller.UpdateUserPreference(7, request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(updatedPreference, okResult.Value);
    }

    [Fact]
    public async Task UpdateUserPreference_ReturnsBadRequest_ValidationFails()
    {
        var request = CreatePreference(7);
        var service = new Mock<IUserPreferenceService>();
        var validator = new Mock<IValidator<UserPreferenceDTO>>();
        validator.Setup(item => item.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure(nameof(UserPreferenceDTO.MinBudget), "Minimum budget cannot be greater than maximum budget.")
            }));
        var controller = CreateController(service.Object, validator.Object);

        var result = await controller.UpdateUserPreference(7, request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Single(Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequest.Value));
        service.Verify(item => item.UpdateUserPreferenceAsync(It.IsAny<int>(), It.IsAny<UserPreferenceDTO>()), Times.Never);
    }

    private static UserPreferenceController CreateController(
        IUserPreferenceService service,
        IValidator<UserPreferenceDTO>? validator = null)
    {
        validator ??= CreatePassingValidator();
        return new UserPreferenceController(service, validator);
    }

    private static IValidator<UserPreferenceDTO> CreatePassingValidator()
    {
        var validator = new Mock<IValidator<UserPreferenceDTO>>();
        validator.Setup(item => item.ValidateAsync(It.IsAny<UserPreferenceDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        return validator.Object;
    }

    private static UserPreferenceDTO CreatePreference(int userId)
    {
        return new UserPreferenceDTO
        {
            UserId = userId,
            MinBudget = 10m,
            MaxBudget = 100m,
            MinPlayTime = 15,
            MaxPlayTime = 120,
            MinPlayers = 1,
            MaxPlayers = 4,
            AgeGroup = "Family",
            GenreIds = [1],
            TypeIds = [1]
        };
    }
}
