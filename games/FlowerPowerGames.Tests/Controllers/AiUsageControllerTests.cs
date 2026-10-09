using FlowerPowerGames.API.Controllers;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Business.Validators;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FlowerPowerGames.Tests.Controllers;

public class AiUsageControllerTests
{
    [Fact]
    public async Task GetAiUsageByUserId_ReturnsOkWithUsage()
    {
        var usage = CreateDto(7);
        var service = new Mock<IAiUsageService>();
        service.Setup(item => item.GetAiUsageByUserIdAsync(7)).ReturnsAsync(usage);
        var controller = CreateController(service.Object);

        var result = await controller.GetAiUsageByUserId(7);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(usage, okResult.Value);
        service.Verify(item => item.GetAiUsageByUserIdAsync(7), Times.Once);
    }

    [Fact]
    public async Task GetAiUsageByUserId_PropagatesNotFoundException()
    {
        var service = new Mock<IAiUsageService>();
        service.Setup(item => item.GetAiUsageByUserIdAsync(42))
            .ThrowsAsync(new NotFoundException("AI usage for user with ID 42 does not exist."));
        var controller = CreateController(service.Object);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            controller.GetAiUsageByUserId(42));

        Assert.Equal("AI usage for user with ID 42 does not exist.", exception.Message);
    }

    [Fact]
    public async Task GetAiUsageById_ReturnsOkWithUsage()
    {
        var usage = CreateDto(7);
        usage.Id = 11;
        var service = new Mock<IAiUsageService>();
        service.Setup(item => item.GetAiUsageByIdAsync(11)).ReturnsAsync(usage);
        var controller = CreateController(service.Object);

        var result = await controller.GetAiUsageById(11);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(usage, okResult.Value);
    }

    [Fact]
    public async Task CreateAiUsage_ReturnsCreatedAtAction()
    {
        var request = CreateCreateDto(750);
        var createdUsage = CreateDto(7);
        createdUsage.Id = 11;
        var service = new Mock<IAiUsageService>();
        service.Setup(item => item.CreateAiUsageAsync(request, 7)).ReturnsAsync(createdUsage);
        var controller = CreateController(service.Object);

        var result = await controller.CreateAiUsage(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(AiUsageController.GetAiUsageById), createdResult.ActionName);
        Assert.Equal(11, createdResult.RouteValues!["id"]);
        Assert.Same(createdUsage, createdResult.Value);
        service.Verify(item => item.CreateAiUsageAsync(request, 7), Times.Once);
    }

    [Fact]
    public async Task UpdateAiUsage_ReturnsOkWithUpdatedUsage()
    {
        var request = CreateDto(7);
        var updatedUsage = CreateDto(7);
        updatedUsage.TotalRequests = 8;
        var service = new Mock<IAiUsageService>();
        service.Setup(item => item.UpdateAiUsageAsync(11, request)).ReturnsAsync(updatedUsage);
        var controller = CreateController(service.Object);

        var result = await controller.UpdateAiUsage(11, request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(updatedUsage, okResult.Value);
    }

    private static AiUsageController CreateController(IAiUsageService service)
    {
        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.SetupGet(item => item.UserId).Returns(7);
        return new AiUsageController(
            service,
            new AiUsageDTOValidator(),
            new CreateAiUsageDTOValidator(),
            new UpdateAiUsageDTOValidator(),
            currentUserService.Object);
    }

    private static CreateAiUsageDTO CreateCreateDto(int totalAvailablePrompt)
    {
        return new CreateAiUsageDTO
        {
            TotalAvailablePrompt = totalAvailablePrompt
        };
    }

    private static AiUsageDTO CreateDto(int userId)
    {
        return new AiUsageDTO
        {
            UserId = userId,
            TotalRequests = 3,
            TotalPromptUsed = 125,
            TotalAvailablePrompt = 750
        };
    }
}
