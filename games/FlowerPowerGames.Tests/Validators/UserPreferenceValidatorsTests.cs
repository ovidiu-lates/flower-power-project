using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Validators;
using Xunit;

namespace FlowerPowerGames.Tests.Validators;

public class UserPreferenceDTOValidatorTests
{
    private readonly UserPreferenceDtoValidator _validator = new();

    [Fact]
    public void Validate_AcceptsValidPreferenceAtMinimumBoundaries()
    {
        var result = _validator.Validate(new UserPreferenceDto
        {
            UserId = 1,
            MinBudget = 0,
            MaxBudget = 0,
            MinPlayTime = 0,
            MaxPlayTime = 0,
            MinPlayers = 1,
            MaxPlayers = 1,
            MinimumAge = 0,
            GenreIds = [1],
            TypeIds = [1]
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsInvalidScalarValues()
    {
        var result = _validator.Validate(new UserPreferenceDto
        {
            UserId = 0,
            MinBudget = -1,
            MaxBudget = -1,
            MinPlayTime = -1,
            MaxPlayTime = -1,
            MinPlayers = 0,
            MaxPlayers = 0,
            MinimumAge = -1
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDto.UserId));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDto.MinBudget));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDto.MaxBudget));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDto.MinPlayTime));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDto.MaxPlayTime));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDto.MinPlayers));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDto.MaxPlayers));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDto.MinimumAge));
    }

    [Fact]
    public void Validate_RejectsMinimumsGreaterThanMaximums()
    {
        var result = _validator.Validate(new UserPreferenceDto
        {
            UserId = 1,
            MinBudget = 2,
            MaxBudget = 1,
            MinPlayTime = 2,
            MaxPlayTime = 1,
            MinPlayers = 3,
            MaxPlayers = 2,
            GenreIds = [1],
            TypeIds = [1]
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDto.MinBudget));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDto.MinPlayTime));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDto.MinPlayers));
    }

    [Fact]
    public void Validate_RejectsNullCollectionsAndNonPositiveIds()
    {
        var nullCollectionsResult = _validator.Validate(new UserPreferenceDto
        {
            GenreIds = null!,
            TypeIds = null!
        });
        var invalidIdsResult = _validator.Validate(new UserPreferenceDto
        {
            GenreIds = [0],
            TypeIds = [-1]
        });

        Assert.Contains(nullCollectionsResult.Errors, error => error.PropertyName == nameof(UserPreferenceDto.GenreIds));
        Assert.Contains(nullCollectionsResult.Errors, error => error.PropertyName == nameof(UserPreferenceDto.TypeIds));
        Assert.Contains(invalidIdsResult.Errors, error => error.PropertyName.StartsWith(nameof(UserPreferenceDto.GenreIds) + "[", StringComparison.Ordinal));
        Assert.Contains(invalidIdsResult.Errors, error => error.PropertyName.StartsWith(nameof(UserPreferenceDto.TypeIds) + "[", StringComparison.Ordinal));
    }
}

public class CreateUserPreferenceDTOValidatorTests
{
    private readonly CreateUserPreferenceDtoValidator _validator = new();

    [Fact]
    public void Validate_AcceptsValidPreferenceAtMinimumBoundaries()
    {
        var result = _validator.Validate(new CreateUserPreferenceDto
        {
            MinBudget = 0,
            MaxBudget = 0,
            MinPlayTime = 0,
            MaxPlayTime = 0,
            MinPlayers = 1,
            MaxPlayers = 1,
            MinimumAge = 0,
            GenreIds = [1],
            TypeIds = [1]
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsNegativeScalarValues()
    {
        var result = _validator.Validate(new CreateUserPreferenceDto
        {
            MinBudget = -1,
            MaxBudget = -1,
            MinPlayTime = -1,
            MaxPlayTime = -1,
            MinPlayers = 0,
            MaxPlayers = 0,
            MinimumAge = -1
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDto.MinBudget));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDto.MaxBudget));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDto.MinPlayTime));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDto.MaxPlayTime));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDto.MinPlayers));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDto.MaxPlayers));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDto.MinimumAge));
    }

    [Fact]
    public void Validate_RejectsMinimumsGreaterThanMaximums()
    {
        var result = _validator.Validate(new CreateUserPreferenceDto
        {
            MinBudget = 2,
            MaxBudget = 1,
            MinPlayTime = 2,
            MaxPlayTime = 1,
            MinPlayers = 3,
            MaxPlayers = 2,
            GenreIds = [1],
            TypeIds = [1]
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDto.MinBudget));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDto.MinPlayTime));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDto.MinPlayers));
    }

    [Fact]
    public void Validate_RejectsNullCollectionsAndNonPositiveIds()
    {
        var nullCollectionsResult = _validator.Validate(new CreateUserPreferenceDto
        {
            GenreIds = null!,
            TypeIds = null!
        });
        var invalidIdsResult = _validator.Validate(new CreateUserPreferenceDto
        {
            GenreIds = [0],
            TypeIds = [-1]
        });

        Assert.Contains(nullCollectionsResult.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDto.GenreIds));
        Assert.Contains(nullCollectionsResult.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDto.TypeIds));
        Assert.Contains(invalidIdsResult.Errors, error => error.PropertyName.StartsWith(nameof(CreateUserPreferenceDto.GenreIds) + "[", StringComparison.Ordinal));
        Assert.Contains(invalidIdsResult.Errors, error => error.PropertyName.StartsWith(nameof(CreateUserPreferenceDto.TypeIds) + "[", StringComparison.Ordinal));
    }
}
