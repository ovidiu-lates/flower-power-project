using FluentValidation;
using FlowerPowerGames.Business.DTOs;
namespace FlowerPowerGames.Business.Validators;

public class CreateAiUsageDtoValidator : AbstractValidator<CreateAiUsageDto>
{
    public CreateAiUsageDtoValidator()
    {
        RuleFor(x => x.TotalAvailablePrompt)
            .GreaterThanOrEqualTo(0)
            .WithMessage("TotalAvailablePrompt must be non-negative.");
    }
}
