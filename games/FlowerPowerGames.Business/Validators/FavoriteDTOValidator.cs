using FlowerPowerGames.Business.DTOs;
using FluentValidation;

namespace FlowerPowerGames.Business.Validators
{
    public class FavoriteDtoValidator : AbstractValidator<FavoriteDto>
    {
        public FavoriteDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be a positive integer.");

            RuleFor(x => x.GameId)
                .GreaterThan(0)
                .WithMessage("GameId must be a positive integer.");
        }
    }
}
