using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Validators;
using Xunit;

namespace FlowerPowerGames.Tests.Validators;

public class AiUsageDTOValidatorTests
{
    private readonly AiUsageDTOValidator _validator = new();

    [Fact]
    public void Validate_AcceptsNonNegativeValuesWithinPromptLimit()
    {
        var result = _validator.Validate(new AiUsageDTO
        {
            TotalRequests = 0,
            TotalPromptUsed = 10,
            TotalAvailablePrompt = 10
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsNegativeCountersAndAvailablePrompt()
    {
        var result = _validator.Validate(new AiUsageDTO
        {
            TotalRequests = -1,
            TotalPromptUsed = -1,
            TotalAvailablePrompt = -1
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AiUsageDTO.TotalRequests));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AiUsageDTO.TotalPromptUsed));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AiUsageDTO.TotalAvailablePrompt));
    }

    [Fact]
    public void Validate_RejectsPromptUsageAboveAvailableLimit()
    {
        var result = _validator.Validate(new AiUsageDTO
        {
            TotalPromptUsed = 11,
            TotalAvailablePrompt = 10
        });

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(AiUsageDTO.TotalPromptUsed) &&
            error.ErrorMessage == "TotalPromptUsed cannot exceed TotalAvailablePrompt.");
    }
}

public class CreateAiUsageDTOValidatorTests
{
    private readonly CreateAiUsageDTOValidator _validator = new();

    [Fact]
    public void Validate_AcceptsZeroAvailablePrompt()
    {
        var result = _validator.Validate(new CreateAiUsageDTO { TotalAvailablePrompt = 0 });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsNegativeAvailablePrompt()
    {
        var result = _validator.Validate(new CreateAiUsageDTO { TotalAvailablePrompt = -1 });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateAiUsageDTO.TotalAvailablePrompt));
    }
}

public class UpdateAiUsageDTOValidatorTests
{
    private readonly UpdateAiUsageDTOValidator _validator = new();

    [Fact]
    public void Validate_AcceptsZeroAvailablePrompt()
    {
        var result = _validator.Validate(new UpdateAiUsageDTO { TotalAvailablePrompt = 0 });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsNegativeAvailablePrompt()
    {
        var result = _validator.Validate(new UpdateAiUsageDTO { TotalAvailablePrompt = -1 });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateAiUsageDTO.TotalAvailablePrompt));
    }
}
