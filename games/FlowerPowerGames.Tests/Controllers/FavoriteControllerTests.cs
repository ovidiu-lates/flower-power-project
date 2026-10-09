using System.Security.Claims;
using FlowerPowerGames.API.Controllers;
using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FlowerPowerGames.UnitTests.Controllers;

public class FavoriteControllerTests
{
    private static FavoriteController CreateController(
        int? userId = 10,
        string? role = "User",
        bool validationSucceeds = true)
    {
        var favoriteService = new Mock<IFavoriteService>();
        var validator = new Mock<IValidator<CreateFavoriteDTO>>();
        var currentUserService = new Mock<ICurrentUserService>();

        currentUserService.SetupGet(service => service.UserId).Returns(userId);
        currentUserService.SetupGet(service => service.Role).Returns(role);
        currentUserService.SetupGet(service => service.IsAuthenticated).Returns(userId.HasValue);
        currentUserService.SetupGet(service => service.Username).Returns("test-user");

        var validationResult = validationSucceeds
            ? new ValidationResult()
            : new ValidationResult([
                new ValidationFailure("GameId", "GameId must be a positive integer.")]);

        validator
            .Setup(service => service.ValidateAsync(
                It.IsAny<CreateFavoriteDTO>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        return new FavoriteController(
            favoriteService.Object,
            validator.Object,
            currentUserService.Object);
    }

    [Fact]
    public async Task CreateFavorite_WhenAuthenticatedUsesAuthenticatedUserId()
    {
        var favoriteService = new Mock<IFavoriteService>();
        var validator = CreateValidValidator();
        var currentUserService = CreateCurrentUserService(10, "User");
        var controller = new FavoriteController(
            favoriteService.Object,
            validator.Object,
            currentUserService.Object);
        var request = new CreateFavoriteDTO { GameId = 1 };
        var created = new FavoriteDTO { Id = 5, UserId = 10, GameId = 1 };
        favoriteService
            .Setup(service => service.CreateFavoriteAsync(request, 10))
            .ReturnsAsync(created);

        var result = await controller.CreateFavorite(request);

        var response = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(FavoriteController.GetFavoriteById), response.ActionName);
        Assert.Same(created, response.Value);
        favoriteService.Verify(
            service => service.CreateFavoriteAsync(request, 10),
            Times.Once);
    }

    [Fact]
    public async Task CreateFavorite_WhenUnauthenticatedThrowsUnauthorizedException()
    {
        var favoriteService = new Mock<IFavoriteService>();
        var validator = CreateValidValidator();
        var currentUserService = CreateCurrentUserService(null, null);
        var controller = new FavoriteController(
            favoriteService.Object,
            validator.Object,
            currentUserService.Object);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(
            () => controller.CreateFavorite(new CreateFavoriteDTO { GameId = 1 }));

        Assert.Equal("User is not authenticated.", exception.Message);
        favoriteService.Verify(
            service => service.CreateFavoriteAsync(
                It.IsAny<CreateFavoriteDTO>(),
                It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateFavorite_WhenRequestIsInvalidReturnsBadRequest()
    {
        var favoriteService = new Mock<IFavoriteService>();
        var validator = CreateInvalidValidator();
        var currentUserService = CreateCurrentUserService(10, "User");
        var controller = new FavoriteController(
            favoriteService.Object,
            validator.Object,
            currentUserService.Object);

        var result = await controller.CreateFavorite(new CreateFavoriteDTO { GameId = 0 });

        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(response.Value);
        favoriteService.Verify(
            service => service.CreateFavoriteAsync(
                It.IsAny<CreateFavoriteDTO>(),
                It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task GetMyFavorites_UsesAuthenticatedUserId()
    {
        var favoriteService = new Mock<IFavoriteService>();
        var validator = CreateValidValidator();
        var currentUserService = CreateCurrentUserService(10, "User");
        var favorites = new List<FavoriteDTO>
        {
            new() { Id = 1, UserId = 10, GameId = 1 }
        };
        favoriteService
            .Setup(service => service.GetFavoritesByUserIdAsync(10))
            .ReturnsAsync(favorites);
        var controller = new FavoriteController(
            favoriteService.Object,
            validator.Object,
            currentUserService.Object);

        var result = await controller.GetMyFavorites();

        var response = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(favorites, response.Value);
        favoriteService.Verify(
            service => service.GetFavoritesByUserIdAsync(10),
            Times.Once);
    }

    [Fact]
    public async Task GetMyFavorites_WhenUnauthenticatedThrowsUnauthorizedException()
    {
        var controller = new FavoriteController(
            new Mock<IFavoriteService>().Object,
            CreateValidValidator().Object,
            CreateCurrentUserService(null, null).Object);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(
            () => controller.GetMyFavorites());

        Assert.Equal("User is not authenticated.", exception.Message);
    }

    [Fact]
    public async Task DeleteFavorite_PassesAuthenticatedUserAndRoleToService()
    {
        var favoriteService = new Mock<IFavoriteService>();
        favoriteService
            .Setup(service => service.DeleteFavoriteAsync(5, 10, "User"))
            .ReturnsAsync(true);
        var controller = new FavoriteController(
            favoriteService.Object,
            CreateValidValidator().Object,
            CreateCurrentUserService(10, "User").Object);

        var result = await controller.DeleteFavorite(5);

        Assert.IsType<NoContentResult>(result);
        favoriteService.Verify(
            service => service.DeleteFavoriteAsync(5, 10, "User"),
            Times.Once);
    }

    [Fact]
    public async Task DeleteFavorite_WhenRoleIsMissingThrowsUnauthorizedException()
    {
        var controller = new FavoriteController(
            new Mock<IFavoriteService>().Object,
            CreateValidValidator().Object,
            CreateCurrentUserService(10, null).Object);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(
            () => controller.DeleteFavorite(5));

        Assert.Equal("User role is not authenticated.", exception.Message);
    }

    private static Mock<IValidator<CreateFavoriteDTO>> CreateValidValidator()
    {
        var validator = new Mock<IValidator<CreateFavoriteDTO>>();
        validator
            .Setup(service => service.ValidateAsync(
                It.IsAny<CreateFavoriteDTO>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        return validator;
    }

    private static Mock<IValidator<CreateFavoriteDTO>> CreateInvalidValidator()
    {
        var validator = new Mock<IValidator<CreateFavoriteDTO>>();
        validator
            .Setup(service => service.ValidateAsync(
                It.IsAny<CreateFavoriteDTO>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([
                new ValidationFailure("GameId", "GameId must be a positive integer.")]));
        return validator;
    }

    private static Mock<ICurrentUserService> CreateCurrentUserService(
        int? userId,
        string? role)
    {
        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.SetupGet(service => service.UserId).Returns(userId);
        currentUserService.SetupGet(service => service.Role).Returns(role);
        currentUserService.SetupGet(service => service.IsAuthenticated).Returns(userId.HasValue);
        currentUserService.SetupGet(service => service.Username).Returns("test-user");
        return currentUserService;
    }
}
