using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Validators;
using Xunit;

namespace FlowerPowerGames.Tests.Validators;

public class AiUsageDTOValidatorTests
{
    private readonly AiUsageDtoValidator _validator = new();

    [Fact]
    public void Validate_AcceptsNonNegativeValuesWithinPromptLimit()
    {
        var result = _validator.Validate(
            new AiUsageDto
            {
                UserId = 1,
                TotalRequests = 0,
                TotalPromptUsed = 10,
                TotalAvailablePrompt = 10
            });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsNegativeCountersAndAvailablePrompt()
    {
        var result = _validator.Validate(new AiUsageDto
        {
            TotalRequests = -1,
            TotalPromptUsed = -1,
            TotalAvailablePrompt = -1
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AiUsageDto.TotalRequests));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AiUsageDto.TotalPromptUsed));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AiUsageDto.TotalAvailablePrompt));
    }

    [Fact]
    public void Validate_RejectsPromptUsageAboveAvailableLimit()
    {
        var result = _validator.Validate(new AiUsageDto
        {
            TotalPromptUsed = 11,
            TotalAvailablePrompt = 10
        });

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(AiUsageDto.TotalPromptUsed) &&
            error.ErrorMessage == "TotalPromptUsed cannot exceed TotalAvailablePrompt.");
    }
}

public class CreateAiUsageDTOValidatorTests
{
    private readonly CreateAiUsageDtoValidator _validator = new();

    [Fact]
    public void Validate_AcceptsZeroAvailablePrompt()
    {
        var result = _validator.Validate(new CreateAiUsageDto { TotalAvailablePrompt = 0 });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsNegativeAvailablePrompt()
    {
        var result = _validator.Validate(new CreateAiUsageDto { TotalAvailablePrompt = -1 });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateAiUsageDto.TotalAvailablePrompt));
    }
}

public class UpdateAiUsageDTOValidatorTests
{
    private readonly UpdateAiUsageDtoValidator _validator = new();

    [Fact]
    public void Validate_AcceptsZeroAvailablePrompt()
    {
        var result = _validator.Validate(new UpdateAiUsageDto { TotalAvailablePrompt = 0 });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsNegativeAvailablePrompt()
    {
        var result = _validator.Validate(new UpdateAiUsageDto { TotalAvailablePrompt = -1 });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateAiUsageDto.TotalAvailablePrompt));
    }
}
