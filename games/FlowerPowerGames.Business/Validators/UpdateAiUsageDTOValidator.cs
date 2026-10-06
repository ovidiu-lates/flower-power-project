using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Validators;

public class UpdateAiUsageDTOValidator : AbstractValidator<UpdateAiUsageDTO>
{
    public UpdateAiUsageDTOValidator()
    {
        RuleFor(x => x.TotalAvailablePrompt)
            .GreaterThanOrEqualTo(0)
            .WithMessage("TotalAvailablePrompt must be non-negative.");
    }
}
