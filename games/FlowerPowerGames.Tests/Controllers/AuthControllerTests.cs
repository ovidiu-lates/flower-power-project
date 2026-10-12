using FlowerPowerGames.API.Controllers;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Business.Validators;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FlowerPowerGames.Tests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_ReturnsCreatedResponse()
    {
        var request = CreateRegistrationRequest();
        var response = new RegisterResponseDto
        {
            UserId = 12,
            Email = request.Email,
            Username = request.Username,
            FullName = request.FullName,
            Role = "User"
        };
        var service = new Mock<IAuthService>();
        service.Setup(item => item.RegisterAsync(request)).ReturnsAsync(response);
        var controller = CreateController(service.Object);

        var result = await controller.Register(request);

        var createdResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Same(response, createdResult.Value);
        service.Verify(item => item.RegisterAsync(request), Times.Once);
    }

    [Fact]
    public async Task Login_ReturnsOkWithLoginResponse()
    {
        var request = new LoginRequestDto { EmailOrUsername = "player", Password = "password123" };
        var response = new LoginResponseDto { UserId = 12, Username = "player" };
        var service = new Mock<IAuthService>();
        service.Setup(item => item.LoginAsync(request)).ReturnsAsync(response);
        var controller = CreateController(service.Object);

        var result = await controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, okResult.Value);
        service.Verify(item => item.LoginAsync(request), Times.Once);
    }

    [Fact]
    public async Task Refresh_ReturnsOkWithRotatedLoginResponse()
    {
        var request = new RefreshTokenRequestDto { RefreshToken = "valid-refresh-token" };
        var response = new LoginResponseDto { UserId = 12, RefreshToken = "rotated-refresh-token" };
        var service = new Mock<IAuthService>();
        service.Setup(item => item.RefreshAsync(request.RefreshToken)).ReturnsAsync(response);
        var controller = CreateController(service.Object);

        var result = await controller.Refresh(request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, okResult.Value);
        service.Verify(item => item.RefreshAsync(request.RefreshToken), Times.Once);
    }

    [Fact]
    public async Task Logout_ReturnsNoContentAndCallsService()
    {
        var request = new RefreshTokenRequestDto { RefreshToken = "valid-refresh-token" };
        var service = new Mock<IAuthService>();
        service.Setup(item => item.LogoutAsync(request.RefreshToken)).ReturnsAsync(true);
        var controller = CreateController(service.Object);

        var result = await controller.Logout(request);

        Assert.IsType<NoContentResult>(result);
        service.Verify(item => item.LogoutAsync(request.RefreshToken), Times.Once);
    }

    [Fact]
    public async Task Login_PropagatesAuthenticationFailure()
    {
        var request = new LoginRequestDto { EmailOrUsername = "player", Password = "wrong-password" };
        var service = new Mock<IAuthService>();
        service.Setup(item => item.LoginAsync(request))
            .ThrowsAsync(new FlowerPowerGames.Business.Exceptions.UnauthorizedException("Invalid username/email or password."));
        var controller = CreateController(service.Object);

        await Assert.ThrowsAsync<FlowerPowerGames.Business.Exceptions.UnauthorizedException>(() => controller.Login(request));
    }

    private static AuthController CreateController(IAuthService service)
    {
        return new AuthController(
            service,
            new RegisterRequestDtoValidator(),
            new LoginRequestDtoValidator(),
            new RefreshTokenRequestDtoValidator());
    }

    private static RegisterRequestDto CreateRegistrationRequest()
    {
        return new RegisterRequestDto
        {
            Email = "player@example.com",
            Username = "player",
            FullName = "Test Player",
            Password = "password123",
            ConfirmPassword = "password123"
        };
    }
}
