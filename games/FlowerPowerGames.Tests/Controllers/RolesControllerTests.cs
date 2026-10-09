using FlowerPowerGames.API.Controllers;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Business.Validators;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FlowerPowerGames.Tests.Controllers;

public class RolesControllerTests
{
    [Fact]
    public async Task GetAllRoles_ReturnOkWithRoles_ServiceSucceeds()
    {
        var roles = new List<RoleDto>
        {
            new() { RoleId = 1, Name = "Admin" },
            new() { RoleId = 2, Name = "User" }
        };
        var service = new Mock<IRoleService>();
        service.Setup(item => item.GetAllRolesAsync()).ReturnsAsync(roles);
        var controller = CreateController(service.Object);

        var result = await controller.GetAllRoles();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(roles, okResult.Value);
        service.Verify(item => item.GetAllRolesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetRoleById_ReturnOkWithRole_RoleExists()
    {
        var role = new RoleDto { RoleId = 4, Name = "Moderator" };
        var service = new Mock<IRoleService>();
        service.Setup(item => item.GetRoleByIdAsync(4)).ReturnsAsync(role);
        var controller = CreateController(service.Object);

        var result = await controller.GetRoleById(4);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(role, okResult.Value);
    }

    [Fact]
    public async Task CreateRole_ReturnsCreatedAtAction()
    {
        var roleDto = new RoleDto { Name = "Editor" };
        var createdRole = new RoleDto { RoleId = 7, Name = "Editor" };
        var service = new Mock<IRoleService>();
        service.Setup(item => item.CreateRoleAsync(roleDto)).ReturnsAsync(createdRole);
        var controller = CreateController(service.Object);

        var result = await controller.CreateRole(roleDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(RolesController.GetRoleById), createdResult.ActionName);
        Assert.Equal(7, createdResult.RouteValues!["id"]);
        Assert.Same(createdRole, createdResult.Value);
    }

    [Fact]
    public async Task UpdateRole_ReturnOkWithUpdatedRole_ServiceSucceeds()
    {
        var roleDto = new RoleDto { RoleId = 5, Name = "Editor" };
        var service = new Mock<IRoleService>();
        service.Setup(item => item.UpdateRoleAsync(5, roleDto)).ReturnsAsync(roleDto);
        var controller = CreateController(service.Object);

        var result = await controller.UpdateRole(5, roleDto);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(roleDto, okResult.Value);
    }

    [Fact]
    public async Task DeleteRole_ReturnNoContent_ServiceReturnsFalse()
    {
        var service = new Mock<IRoleService>();
        service.Setup(item => item.DeleteRoleAsync(9)).ReturnsAsync(false);
        var controller = CreateController(service.Object);

        var result = await controller.DeleteRole(9);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetAllRoles_PropagateServiceException_ServiceFails()
    {
        var service = new Mock<IRoleService>();
        service.Setup(item => item.GetAllRolesAsync())
            .ThrowsAsync(new InvalidOperationException("Service failure."));
        var controller = CreateController(service.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => controller.GetAllRoles());
    }

    private static RolesController CreateController(IRoleService service)
    {
        return new RolesController(service, new RoleDtoValidator());
    }
}