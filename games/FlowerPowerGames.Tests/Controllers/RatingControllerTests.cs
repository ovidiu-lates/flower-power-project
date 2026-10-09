using FlowerPowerGames.API.Controllers;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FlowerPowerGames.UnitTests.Controllers;

public class RatingControllerTests
{
    private static RatingController CreateController(
        out Mock<IRatingService> ratingService,
        out Mock<IValidator<RatingDto>> ratingValidator,
        out Mock<IValidator<CreateRatingDto>> createValidator,
        out Mock<ICurrentUserService> currentUserService)
    {
        ratingService = new Mock<IRatingService>();
        ratingValidator = new Mock<IValidator<RatingDto>>();
        createValidator = new Mock<IValidator<CreateRatingDto>>();
        currentUserService = new Mock<ICurrentUserService>();

        currentUserService
            .SetupGet(service => service.UserId)
            .Returns(10);

        ratingValidator
            .Setup(validator =>
                validator.ValidateAsync(
                    It.IsAny<RatingDto>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        createValidator
            .Setup(validator =>
                validator.ValidateAsync(
                    It.IsAny<CreateRatingDto>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        return new RatingController(
            ratingService.Object,
            ratingValidator.Object,
            createValidator.Object,
            currentUserService.Object);
    }

    [Fact]
    public async Task GetAllRatings_ReturnsOkWithList()
    {
        var controller = CreateController(
            out var service,
            out _,
            out _,
            out _);

        var ratings = new List<RatingDto>
        {
            new()
            {
                Id = 1,
                UserId = 10,
                GameId = 5,
                Score = 8,
                Review = "Good game"
            }
        };

        service
            .Setup(x => x.GetAllRatingsAsync())
            .ReturnsAsync(ratings);

        var result = await controller.GetAllRatings();

        var ok = Assert.IsType<OkObjectResult>(
            result.Result);

        var returned =
            Assert.IsAssignableFrom<IEnumerable<RatingDto>>(
                ok.Value);

        Assert.Single(returned);
    }

    [Fact]
    public async Task GetMyRatings_ReturnsRatingsForCurrentUser()
    {
        var controller = CreateController(
            out var service,
            out _,
            out _,
            out _);

        var ratings = new List<RatingDto>
        {
            new()
            {
                Id = 1,
                UserId = 10,
                GameId = 5,
                Score = 9
            }
        };

        service
            .Setup(x => x.GetRatingsByUserIdAsync(10))
            .ReturnsAsync(ratings);

        var result = await controller.GetMyRatings();

        var ok = Assert.IsType<OkObjectResult>(
            result.Result);

        var returned =
            Assert.IsAssignableFrom<IEnumerable<RatingDto>>(
                ok.Value);

        Assert.Single(returned);
    }

    [Fact]
    public async Task GetMyRatings_WhenUserIsNotAuthenticated_ThrowsUnauthorized()
    {
        var controller = CreateController(
            out _,
            out _,
            out _,
            out var currentUserService);

        currentUserService
            .SetupGet(service => service.UserId)
            .Returns((int?)null);

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => controller.GetMyRatings());
    }

    [Fact]
    public async Task GetRatingById_ReturnsOk()
    {
        var controller = CreateController(
            out var service,
            out _,
            out _,
            out _);

        var rating = new RatingDto
        {
            Id = 1,
            UserId = 10,
            GameId = 5,
            Score = 8
        };

        service
            .Setup(x => x.GetRatingByIdAsync(1))
            .ReturnsAsync(rating);

        var result = await controller.GetRatingById(1);

        var ok = Assert.IsType<OkObjectResult>(
            result.Result);

        var returned = Assert.IsType<RatingDto>(
            ok.Value);

        Assert.Equal(1, returned.Id);
    }

    [Fact]
    public async Task GetRatingById_WhenMissing_ThrowsNotFound()
    {
        var controller = CreateController(
            out var service,
            out _,
            out _,
            out _);

        service
            .Setup(x => x.GetRatingByIdAsync(99))
            .ThrowsAsync(
                new NotFoundException(
                    "Rating was not found."));

        await Assert.ThrowsAsync<NotFoundException>(
            () => controller.GetRatingById(99));
    }

    [Fact]
    public async Task CreateRating_ReturnsCreatedAtAction()
    {
        var controller = CreateController(
            out var service,
            out _,
            out _,
            out _);

        var createdRating = new RatingDto
        {
            Id = 7,
            UserId = 10,
            GameId = 5,
            Score = 9,
            Review = "Excellent"
        };

        service
            .Setup(x =>
                x.CreateRatingAsync(
                    It.IsAny<CreateRatingDto>(),
                    10))
            .ReturnsAsync(createdRating);

        var request = new CreateRatingDto
        {
            GameId = 5,
            Score = 9,
            Review = "Excellent"
        };

        var result =
            await controller.CreateRating(request);

        var created =
            Assert.IsType<CreatedAtActionResult>(
                result.Result);

        Assert.Equal(
            nameof(RatingController.GetRatingById),
            created.ActionName);

        var returned = Assert.IsType<RatingDto>(
            created.Value);

        Assert.Equal(7, returned.Id);
    }

    [Fact]
    public async Task CreateRating_WhenValidationFails_ReturnsBadRequest()
    {
        var controller = CreateController(
            out _,
            out _,
            out var validator,
            out _);

        var validationResult = new ValidationResult(
        [
            new ValidationFailure(
                "Score",
                "Score must be between 1 and 10.")
        ]);

        validator
            .Setup(x =>
                x.ValidateAsync(
                    It.IsAny<CreateRatingDto>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var request = new CreateRatingDto
        {
            GameId = 5,
            Score = 20
        };

        var result =
            await controller.CreateRating(request);

        Assert.IsType<BadRequestObjectResult>(
            result.Result);
    }

    [Fact]
    public async Task UpdateRating_ReturnsOk()
    {
        var controller = CreateController(
            out var service,
            out _,
            out _,
            out _);

        var updatedRating = new RatingDto
        {
            Id = 1,
            UserId = 10,
            GameId = 5,
            Score = 10,
            Review = "Updated"
        };

        service
            .Setup(x =>
                x.UpdateRatingAsync(
                    1,
                    It.IsAny<RatingDto>()))
            .ReturnsAsync(updatedRating);

        var request = new RatingDto
        {
            Id = 1,
            UserId = 10,
            GameId = 5,
            Score = 10,
            Review = "Updated"
        };

        var result =
            await controller.UpdateRating(1, request);

        var ok = Assert.IsType<OkObjectResult>(
            result.Result);

        var returned = Assert.IsType<RatingDto>(
            ok.Value);

        Assert.Equal(10, returned.Score);
    }

    [Fact]
    public async Task DeleteRating_ReturnsNoContent()
    {
        var controller = CreateController(
            out var service,
            out _,
            out _,
            out _);

        service
            .Setup(x => x.DeleteRatingAsync(1))
            .ReturnsAsync(true);

        var result =
            await controller.DeleteRating(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteRating_WhenMissing_ThrowsNotFound()
    {
        var controller = CreateController(
            out var service,
            out _,
            out _,
            out _);

        service
            .Setup(x => x.DeleteRatingAsync(99))
            .ThrowsAsync(
                new NotFoundException(
                    "Rating was not found."));

        await Assert.ThrowsAsync<NotFoundException>(
            () => controller.DeleteRating(99));
    }
}