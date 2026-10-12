using FlowerPowerGames.Business.DTOs;
using FluentValidation;

namespace FlowerPowerGames.Business.Validators;

public class RecommendationRequestValidator : AbstractValidator<RecommendationRequestDto>
{
    public RecommendationRequestValidator()
    {
        RuleFor(x => x.Message)
                .NotEmpty()
                .WithMessage("Recommendation message is required.")
                .MaximumLength(500)
                .WithMessage("recommendation message must not exceed 500 characters.");
    }
}
