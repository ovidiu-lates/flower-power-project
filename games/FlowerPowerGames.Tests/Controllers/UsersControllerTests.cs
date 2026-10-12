using FlowerPowerGames.API.Controllers;
using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FlowerPowerGames.Tests.Controllers;

public class UsersControllerTests
{
    [Fact]
    public async Task GetAllUsers_ReturnsOkWithUsers_ServiceSucceeds()
    {
        var users = new List<UserDto>
        {
            new() { Id = 1, Email = "jane@example.com", Username = "jane", FullName = "Jane Doe" },
            new() { Id = 2, Email = "john@example.com", Username = "john", FullName = "John Doe" }
        };
        var service = new Mock<IUserService>();
        service.Setup(item => item.GetAllUsersAsync()).ReturnsAsync(users);
        var controller = CreateController(service.Object);

        var result = await controller.GetAllUsers();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(users, okResult.Value);
        service.Verify(item => item.GetAllUsersAsync(), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ReturnsOkWithUser_UserExists()
    {
        var user = new UserDto
        {
            Id = 4,
            Email = "jane@example.com",
            Username = "jane",
            FullName = "Jane Doe"
        };
        var service = new Mock<IUserService>();
        service.Setup(item => item.GetUserByIdAsync(4)).ReturnsAsync(user);
        var controller = CreateController(service.Object);

        var result = await controller.GetUserById(4);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(user, okResult.Value);
    }

    [Fact]
    public async Task GetUserById_PropagatesServiceException_UserDoesNotExist()
    {
        var service = new Mock<IUserService>();
        service.Setup(item => item.GetUserByIdAsync(42))
            .ThrowsAsync(new NotFoundException("User with id 42 was not found."));
        var controller = CreateController(service.Object);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => controller.GetUserById(42));

        Assert.Equal("User with id 42 was not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateUser_ReturnsOkWithUpdatedUser()
    {
        var request = new AdminUpdateUserRequestDto { IsActive = false };
        var user = new UserDto
        {
            Id = 4,
            Email = "jane@example.com",
            Username = "jane",
            FullName = "Jane Doe",
            IsActive = false
        };
        var service = new Mock<IUserService>();
        service.Setup(item => item.UpdateAdminUserAsync(4, request)).ReturnsAsync(user);
        var controller = CreateController(service.Object);

        var result = await controller.UpdateUser(4, request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(user, okResult.Value);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent_ServiceSucceeds()
    {
        var service = new Mock<IUserService>();
        service.Setup(item => item.DeleteUserAsync(9)).ReturnsAsync(true);
        var controller = CreateController(service.Object);

        var result = await controller.DeleteUser(9);

        Assert.IsType<NoContentResult>(result);
        service.Verify(item => item.DeleteUserAsync(9), Times.Once);
    }

    [Fact]
    public async Task UpdateMyProfile_ReturnsOkWithUpdatedUser()
    {
        var request = new UpdateProfileRequestDto { FullName = "Jane Updated" };
        var user = new UserDto
        {
            Id = 7,
            Email = "jane@example.com",
            Username = "jane",
            FullName = "Jane Updated"
        };
        var service = new Mock<IUserService>();
        service.Setup(item => item.UpdateMyProfileAsync(7, request)).ReturnsAsync(user);
        var controller = CreateController(service.Object, userId: 7);

        var result = await controller.UpdateMyProfile(request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(user, okResult.Value);
        service.Verify(item => item.UpdateMyProfileAsync(7, request), Times.Once);
    }

    [Fact]
    public async Task UpdateMyProfile_ThrowsUnauthorizedException_CurrentUserIdIsMissing()
    {
        var service = new Mock<IUserService>();
        var controller = CreateController(service.Object, userId: null);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            controller.UpdateMyProfile(new UpdateProfileRequestDto { FullName = "Jane Updated" }));

        service.Verify(item => item.UpdateMyProfileAsync(It.IsAny<int>(), It.IsAny<UpdateProfileRequestDto>()), Times.Never);
    }

    [Fact]
    public async Task ChangePassword_NoContent_ServiceSucceeds()
    {
        var request = new ChangePasswordRequestDto
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword456!",
            ConfirmNewPassword = "NewPassword456!"
        };
        var service = new Mock<IUserService>();
        service.Setup(item => item.ChangePasswordAsync(7, request)).ReturnsAsync(true);
        var controller = CreateController(service.Object, userId: 7);

        var result = await controller.ChangePassword(request);

        Assert.IsType<NoContentResult>(result);
        service.Verify(item => item.ChangePasswordAsync(7, request), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_ThrowsUnauthorizedException_CurrentUserIdIsMissing()
    {
        var service = new Mock<IUserService>();
        var controller = CreateController(service.Object, userId: null);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            controller.ChangePassword(new ChangePasswordRequestDto()));

        service.Verify(item => item.ChangePasswordAsync(It.IsAny<int>(), It.IsAny<ChangePasswordRequestDto>()), Times.Never);
    }

    private static UsersController CreateController(IUserService userService, int? userId = 7)
    {
        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.SetupGet(item => item.UserId).Returns(userId);
        return new UsersController(userService, currentUserService.Object);
    }
}
