using FluentValidation;
using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Validators;

public sealed class UpdateProfileRequestDtoValidator : AbstractValidator<UpdateProfileRequestDto>
{
    public UpdateProfileRequestDtoValidator()
    {
        RuleFor(request => request)
            .Must(request =>
                request.Email is not null ||
                request.Username is not null ||
                request.FullName is not null)
            .WithMessage(
                "At least one field must be provided.");

        RuleFor(request => request.Email)
            .EmailAddress()
            .When(request => request.Email is not null)
            .WithMessage("Invalid email address.");

        RuleFor(request => request.Username)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .When(request => request.Username is not null)
            .WithMessage("Username cannot be empty.");

        RuleFor(request => request.FullName)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .When(request => request.FullName is not null)
            .WithMessage("Full name cannot be empty.");
    }
}