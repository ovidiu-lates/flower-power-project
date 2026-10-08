using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Validators;
using Xunit;

namespace FlowerPowerGames.Tests.Validators;

public class UserPreferenceDTOValidatorTests
{
    private readonly UserPreferenceDTOValidator _validator = new();

    [Fact]
    public void Validate_AcceptsValidPreferenceAtMinimumBoundaries()
    {
        var result = _validator.Validate(new UserPreferenceDTO
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
        var result = _validator.Validate(new UserPreferenceDTO
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

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDTO.UserId));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDTO.MinBudget));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDTO.MaxBudget));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDTO.MinPlayTime));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDTO.MaxPlayTime));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDTO.MinPlayers));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDTO.MaxPlayers));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDTO.MinimumAge));
    }

    [Fact]
    public void Validate_RejectsMinimumsGreaterThanMaximums()
    {
        var result = _validator.Validate(new UserPreferenceDTO
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

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDTO.MinBudget));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDTO.MinPlayTime));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserPreferenceDTO.MinPlayers));
    }

    [Fact]
    public void Validate_RejectsNullCollectionsAndNonPositiveIds()
    {
        var nullCollectionsResult = _validator.Validate(new UserPreferenceDTO
        {
            GenreIds = null!,
            TypeIds = null!
        });
        var invalidIdsResult = _validator.Validate(new UserPreferenceDTO
        {
            GenreIds = [0],
            TypeIds = [-1]
        });

        Assert.Contains(nullCollectionsResult.Errors, error => error.PropertyName == nameof(UserPreferenceDTO.GenreIds));
        Assert.Contains(nullCollectionsResult.Errors, error => error.PropertyName == nameof(UserPreferenceDTO.TypeIds));
        Assert.Contains(invalidIdsResult.Errors, error => error.PropertyName.StartsWith(nameof(UserPreferenceDTO.GenreIds) + "[", StringComparison.Ordinal));
        Assert.Contains(invalidIdsResult.Errors, error => error.PropertyName.StartsWith(nameof(UserPreferenceDTO.TypeIds) + "[", StringComparison.Ordinal));
    }
}

public class CreateUserPreferenceDTOValidatorTests
{
    private readonly CreateUserPreferenceDTOValidator _validator = new();

    [Fact]
    public void Validate_AcceptsValidPreferenceAtMinimumBoundaries()
    {
        var result = _validator.Validate(new CreateUserPreferenceDTO
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
        var result = _validator.Validate(new CreateUserPreferenceDTO
        {
            MinBudget = -1,
            MaxBudget = -1,
            MinPlayTime = -1,
            MaxPlayTime = -1,
            MinPlayers = 0,
            MaxPlayers = 0,
            MinimumAge = -1
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDTO.MinBudget));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDTO.MaxBudget));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDTO.MinPlayTime));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDTO.MaxPlayTime));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDTO.MinPlayers));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDTO.MaxPlayers));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDTO.MinimumAge));
    }

    [Fact]
    public void Validate_RejectsMinimumsGreaterThanMaximums()
    {
        var result = _validator.Validate(new CreateUserPreferenceDTO
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

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDTO.MinBudget));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDTO.MinPlayTime));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDTO.MinPlayers));
    }

    [Fact]
    public void Validate_RejectsNullCollectionsAndNonPositiveIds()
    {
        var nullCollectionsResult = _validator.Validate(new CreateUserPreferenceDTO
        {
            GenreIds = null!,
            TypeIds = null!
        });
        var invalidIdsResult = _validator.Validate(new CreateUserPreferenceDTO
        {
            GenreIds = [0],
            TypeIds = [-1]
        });

        Assert.Contains(nullCollectionsResult.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDTO.GenreIds));
        Assert.Contains(nullCollectionsResult.Errors, error => error.PropertyName == nameof(CreateUserPreferenceDTO.TypeIds));
        Assert.Contains(invalidIdsResult.Errors, error => error.PropertyName.StartsWith(nameof(CreateUserPreferenceDTO.GenreIds) + "[", StringComparison.Ordinal));
        Assert.Contains(invalidIdsResult.Errors, error => error.PropertyName.StartsWith(nameof(CreateUserPreferenceDTO.TypeIds) + "[", StringComparison.Ordinal));
    }
}
