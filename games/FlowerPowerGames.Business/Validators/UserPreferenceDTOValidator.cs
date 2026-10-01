using FlowerPowerGames.Business.DTOs;
using FluentValidation;

namespace FlowerPowerGames.Business.Validators
{
    public class UserPreferenceDTOValidator : AbstractValidator<UserPreferenceDTO>
    {
        public UserPreferenceDTOValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be a positive integer.");

            RuleFor(x => x.MinBudget)
                .GreaterThanOrEqualTo(0)
                .WithMessage("MinBudget must be positive.");

            RuleFor(x => x.MaxBudget)
                .GreaterThanOrEqualTo(0)
                .WithMessage("MaxBudget must be positive.");

            RuleFor(x => x.MinBudget)
                .LessThanOrEqualTo(x => x.MaxBudget)
                .WithMessage("Minimum budget cannot be greater than maximum budget.");

            RuleFor(x => x.MinPlayTime)
                .GreaterThanOrEqualTo(0)
                .WithMessage("MinPlayTime must be positive.");

            RuleFor(x => x.MaxPlayTime)
                .GreaterThanOrEqualTo(0)
                .WithMessage("MaxPlayTime must be positive.");

            RuleFor(x => x.MinPlayTime)
                .LessThanOrEqualTo(x => x.MaxPlayTime)
                .WithMessage("Minimum play time cannot be greater than maximum play time.");

            RuleFor(x => x.MinPlayers)
                .GreaterThanOrEqualTo(1)
                .WithMessage("MinPlayers must be at least 1.");

            RuleFor(x => x.MaxPlayers)
                .GreaterThanOrEqualTo(1)
                .WithMessage("MaxPlayers must be at least 1.");

            RuleFor(x => x.MinPlayers)
                .LessThanOrEqualTo(x => x.MaxPlayers)
                .WithMessage("Minimum players cannot be greater than maximum players.");

            RuleFor(x => x.GenreIds)
                .NotNull()
                .WithMessage("GenreIds cannot be null.");

            RuleForEach(x => x.GenreIds)
                .GreaterThan(0)
                .WithMessage("Genre IDs must be positive.");

            RuleFor(x => x.TypeIds)
                .NotNull()
                .WithMessage("TypeIds cannot be null.");

            RuleForEach(x => x.TypeIds)
                .GreaterThan(0)
                .WithMessage("Type IDs must be positive.");
        }
    }
}