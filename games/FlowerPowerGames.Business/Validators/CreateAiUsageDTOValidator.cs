using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using FlowerPowerGames.Business.DTOs;
namespace FlowerPowerGames.Business.Validators;

public class CreateAiUsageDTOValidator : AbstractValidator<CreateAiUsageDTO>
{
    public CreateAiUsageDTOValidator()
    {
        RuleFor(x => x.TotalAvailablePrompt)
            .GreaterThanOrEqualTo(0)
            .WithMessage("TotalAvailablePrompt must be non-negative.");
    }
}
