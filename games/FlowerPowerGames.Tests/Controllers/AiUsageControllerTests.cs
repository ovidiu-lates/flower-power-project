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
    public async Task CreateAiUsage_ReturnsCreatedAtAction_ValidationSucceeds()
    {
        var request = CreateDto(7);
        var createdUsage = CreateDto(7);
        createdUsage.Id = 11;
        var service = new Mock<IAiUsageService>();
        service.Setup(item => item.CreateAiUsageAsync(request)).ReturnsAsync(createdUsage);
        var controller = CreateController(service.Object);

        var result = await controller.CreateAiUsage(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(AiUsageController.GetAiUsageById), createdResult.ActionName);
        Assert.Equal(11, createdResult.RouteValues!["id"]);
        Assert.Same(createdUsage, createdResult.Value);
    }

    [Fact]
    public async Task CreateAiUsage_ReturnsBadRequest_ValidationFails()
    {
        var request = CreateDto(0);
        var service = new Mock<IAiUsageService>();
        var validator = new Mock<IValidator<AiUsageDTO>>();
        validator.Setup(item => item.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure(nameof(AiUsageDTO.UserId), "UserId must be a positive integer.")
            }));
        var controller = CreateController(service.Object, validator.Object);

        var result = await controller.CreateAiUsage(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Single(Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequest.Value));
        service.Verify(item => item.CreateAiUsageAsync(It.IsAny<AiUsageDTO>()), Times.Never);
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

    [Fact]
    public async Task UpdateAiUsage_ReturnsBadRequest_ValidationFails()
    {
        var request = CreateDto(7);
        var service = new Mock<IAiUsageService>();
        var validator = new Mock<IValidator<AiUsageDTO>>();
        validator.Setup(item => item.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure(nameof(AiUsageDTO.TotalPromptUsed), "TotalPromptUsed cannot exceed TotalAvailablePrompt.")
            }));
        var controller = CreateController(service.Object, validator.Object);

        var result = await controller.UpdateAiUsage(11, request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Single(Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequest.Value));
        service.Verify(item => item.UpdateAiUsageAsync(It.IsAny<int>(), It.IsAny<AiUsageDTO>()), Times.Never);
    }

    private static AiUsageController CreateController(
        IAiUsageService service,
        IValidator<AiUsageDTO>? validator = null)
    {
        validator ??= CreatePassingValidator();
        return new AiUsageController(service, validator);
    }

    private static IValidator<AiUsageDTO> CreatePassingValidator()
    {
        var validator = new Mock<IValidator<AiUsageDTO>>();
        validator.Setup(item => item.ValidateAsync(It.IsAny<AiUsageDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        return validator.Object;
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
