using FlowerPowerGames.Business.DTOs;
using FluentValidation;

namespace FlowerPowerGames.Business.Validators
{
    public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestDtoValidator()
        {
            RuleFor(x => x.EmailOrUsername)
                .NotEmpty()
                .WithMessage("Email or username cannot be empty.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password cannot be empty.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long.");
        }
    }
}
