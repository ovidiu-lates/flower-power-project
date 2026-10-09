using FlowerPowerGames.Business.DTOs;
using FluentValidation;

namespace FlowerPowerGames.Business.Validators;

public sealed class ChangePasswordRequestDtoValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestDtoValidator()
    {
        RuleFor(request => request.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required.");

        RuleFor(request => request.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("New password must contain at least 8 characters.");

        RuleFor(request => request.ConfirmNewPassword)
            .NotEmpty()
            .Equal(request => request.NewPassword)
            .WithMessage("Password does not match.");
    }
}