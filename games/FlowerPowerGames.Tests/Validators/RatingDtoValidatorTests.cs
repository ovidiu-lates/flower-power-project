using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace FlowerPowerGames.Tests.Validators;

public class RatingDtoValidatorTests
{
    private readonly RatingDtoValidator _validator = new();

    [Fact]
    public void ValidRating_DoesNotHaveValidationErrors()
    {
        var rating = new RatingDto
        {
            UserId = 1,
            GameId = 1,
            Score = 8,
            Review = "Good game"
        };

        var result = _validator.TestValidate(rating);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UserIdZero_ReturnsValidationError()
    {
        var rating = new RatingDto
        {
            UserId = 0,
            GameId = 1,
            Score = 8
        };

        var result = _validator.TestValidate(rating);

        result.ShouldHaveValidationErrorFor(
            rating => rating.UserId);
    }

    [Fact]
    public void GameIdZero_ReturnsValidationError()
    {
        var rating = new RatingDto
        {
            UserId = 1,
            GameId = 0,
            Score = 8
        };

        var result = _validator.TestValidate(rating);

        result.ShouldHaveValidationErrorFor(
            rating => rating.GameId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void ScoreOutsideRange_ReturnsValidationError(
        int score)
    {
        var rating = new RatingDto
        {
            UserId = 1,
            GameId = 1,
            Score = score
        };

        var result = _validator.TestValidate(rating);

        result.ShouldHaveValidationErrorFor(
            rating => rating.Score);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void ScoreWithinRange_DoesNotReturnError(
        int score)
    {
        var rating = new RatingDto
        {
            UserId = 1,
            GameId = 1,
            Score = score
        };

        var result = _validator.TestValidate(rating);

        result.ShouldNotHaveValidationErrorFor(
            rating => rating.Score);
    }
}