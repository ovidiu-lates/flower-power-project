using FlowerPowerGames.Business.DTOs;
using FluentValidation;

namespace FlowerPowerGames.Business.Validators
{
    public class GameTypeDtoValidator : AbstractValidator<GameTypeDto>
    {
        public GameTypeDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Game type name cannot be empty.");
        }
    }
}
