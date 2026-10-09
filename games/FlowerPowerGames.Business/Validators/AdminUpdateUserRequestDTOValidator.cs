using FluentValidation;
using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Validators;

public sealed class AdminUpdateUserRequestDtoValidator : AbstractValidator<AdminUpdateUserRequestDto>
{
    public AdminUpdateUserRequestDtoValidator()
    {
        RuleFor(request => request)
            .Must(request => request.RoleId.HasValue || request.IsActive.HasValue)
            .WithMessage("At least one field must be provided.");

        RuleFor(request => request.RoleId)
            .GreaterThan(0)
            .When(request => request.RoleId.HasValue)
            .WithMessage("Role ID must be a positive number.");
    }
}