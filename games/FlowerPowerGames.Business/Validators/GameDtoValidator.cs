using FlowerPowerGames.Business.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;

namespace FlowerPowerGames.Business.Validators
{
    public class GameDtoValidator : AbstractValidator<GameDto>
    {
        public GameDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Game name cannot be empty.");

            RuleFor(x => x.MinPlayers)
                .GreaterThanOrEqualTo(1)
                .WithMessage("MinPlayers must be at least 1.");

            RuleFor(x => x.MaxPlayers)
                .GreaterThanOrEqualTo(1)
                .WithMessage("MaxPlayers must be at least 1.");

            RuleFor(x => x.MinPlayers)
                .LessThanOrEqualTo(x => x.MaxPlayers)
                .WithMessage("MinPlayers cannot be greater than MaxPlayers.");

            RuleFor(x => x.PlayTimeMinutes)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Play time must be at least 1 minute.");

            RuleFor(x => x.LearningTimeMinutes)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Learning time must be non-negative.");

            RuleFor(x => x.MinimumAge)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimum age must be non-negative.");

            RuleFor(x => x.GenreIds)
                .NotNull()
                .Must(list => list != null && list.Count > 0)
                .WithMessage("At least one genre is required.");

            RuleForEach(x => x.GenreIds)
                .GreaterThan(0)
                .WithMessage("Genre IDs must be positive.");

            RuleFor(x => x.TypeIds)
                .NotNull()
                .NotEmpty()
                .WithMessage("At least one type is required.");

            RuleForEach(x => x.TypeIds)
                .GreaterThan(0)
                .WithMessage("Type IDs must be positive.");
        }
    }
}
