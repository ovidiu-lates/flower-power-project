using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Validators;
using Xunit;

namespace FlowerPowerGames.UnitTests.Validators;

public class GameDtoValidatorTests
{
    private static GameDto CreateValidGameDto(
        List<int>? genreIds = null,
        List<int>? typeIds = null)
    {
        return new GameDto
        {
            Name = "Test Game",
            MinPlayers = 1,
            MaxPlayers = 4,
            PlayTimeMinutes = 30,
            LearningTimeMinutes = 10,
            MinimumAge = 8,
            GenreIds = genreIds ?? [1],
            TypeIds = typeIds ?? [1]
        };
    }

    [Fact]
    public async Task ValidateAsync_WhenGenreIdsIsEmptyReturnsValidationError()
    {
        var validator = new GameDtoValidator();

        var result = await validator.ValidateAsync(
            CreateValidGameDto(genreIds: []));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(GameDto.GenreIds) &&
            error.ErrorMessage == "At least one genre is required.");
    }

    [Fact]
    public async Task ValidateAsync_WhenTypeIdsIsEmptyReturnsValidationError()
    {
        var validator = new GameDtoValidator();

        var result = await validator.ValidateAsync(
            CreateValidGameDto(typeIds: []));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(GameDto.TypeIds) &&
            error.ErrorMessage == "At least one type is required.");
    }

    [Fact]
    public async Task ValidateAsync_WhenGenreIdsAndTypeIdsAreEmptyReturnsBothValidationErrors()
    {
        var validator = new GameDtoValidator();

        var result = await validator.ValidateAsync(
            CreateValidGameDto(genreIds: [], typeIds: []));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(GameDto.GenreIds) &&
            error.ErrorMessage == "At least one genre is required.");
        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(GameDto.TypeIds) &&
            error.ErrorMessage == "At least one type is required.");
    }
}
