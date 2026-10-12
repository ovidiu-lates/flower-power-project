using FlowerPowerGames.Business.DTOs;
using FluentValidation;

namespace FlowerPowerGames.Business.Validators
{
    public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
    {
        public RefreshTokenRequestDtoValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token cannot be empty.");
        }
    }
}
