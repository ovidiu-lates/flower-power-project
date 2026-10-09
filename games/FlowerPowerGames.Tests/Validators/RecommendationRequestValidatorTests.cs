using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace FlowerPowerGames.Tests.Validators;

public class RecommendationRequestValidatorTests
{
    private readonly RecommendationRequestValidator _validator = new();

    [Fact]
    public void Validate_WithValidMessage_IsValid()
    {
        var request = new RecommendationRequestDto
        {
            Message = "I want a cooperative board game for four players."
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyMessage_IsInvalid()
    {
        var request = new RecommendationRequestDto
        {
            Message = string.Empty
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Validate_WithMessageLongerThan500Characters_IsInvalid()
    {
        var request = new RecommendationRequestDto
        {
            Message = new string('a', 501)
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Validate_With500CharacterMessage_IsValid()
    {
        var request = new RecommendationRequestDto
        {
            Message = new string('a', 500)
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}