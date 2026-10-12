using FluentValidation;
using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Validators;

public class UpdateAiUsageDtoValidator : AbstractValidator<UpdateAiUsageDto>
{
    public UpdateAiUsageDtoValidator()
    {
        RuleFor(x => x.TotalAvailablePrompt)
            .GreaterThanOrEqualTo(0)
            .WithMessage("TotalAvailablePrompt must be non-negative.");
    }
}
