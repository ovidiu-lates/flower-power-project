using FlowerPowerGames.Business.DTOs;
using FluentValidation;

namespace FlowerPowerGames.Business.Validators;

public class CreateFavoriteDTOValidator: AbstractValidator<CreateFavoriteDTO>
{
    public CreateFavoriteDTOValidator()
    {
        RuleFor(x => x.GameId)
            .GreaterThan(0)
            .WithMessage("GameId must be a positive integer.");
    }
}
