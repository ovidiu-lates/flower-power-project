using FlowerPowerGames.Business.DTOs;
using FluentValidation;

namespace FlowerPowerGames.Business.Validators
{
    public class AiUsageDTOValidator : AbstractValidator<AiUsageDTO>
    {
        public AiUsageDTOValidator()
        {
            /*RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be a positive integer.");*/

            RuleFor(x => x.TotalRequests)
                .GreaterThanOrEqualTo(0)
                .WithMessage("TotalRequests must be non-negative.");

            RuleFor(x => x.TotalAvailablePrompt)
                .GreaterThanOrEqualTo(0)
                .WithMessage("TotalAvailablePrompt must be non-negative.");

            RuleFor(x => x.TotalPromptUsed)
                .GreaterThanOrEqualTo(0)
                .WithMessage("TotalPromptUsed must be non-negative.")
                .LessThanOrEqualTo(x => x.TotalAvailablePrompt)
                .WithMessage("TotalPromptUsed cannot exceed TotalAvailablePrompt.");
        }
    }
}