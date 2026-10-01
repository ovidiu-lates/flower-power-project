using FlowerPowerGames.Business.DTOs;
using FluentValidation;

namespace FlowerPowerGames.Business.Validators
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email cannot be empty.")
                .EmailAddress()
                .WithMessage("Invalid email address.");

            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username cannot be empty.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Full name cannot be empty.");

            RuleFor(x => x.RoleId)
                .GreaterThan(0)
                .WithMessage("Role ID must be a positive number.");
        }
    }
}
