using FlowerPowerGames.Business.DTOs;
using FluentValidation;

namespace FlowerPowerGames.Business.Validators;

public class CreateFavoriteDtoValidator: AbstractValidator<CreateFavoriteDto>
{
    public CreateFavoriteDtoValidator()
    {
        RuleFor(x => x.GameId)
            .GreaterThan(0)
            .WithMessage("GameId must be a positive integer.");
    }
}
