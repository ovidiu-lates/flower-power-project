using FlowerPowerGames.Business.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.Validators
{
    public class RatingDtoValidator : AbstractValidator<RatingDto>
    {
        public RatingDtoValidator()
        {
            RuleFor(x => x.Score)
                .InclusiveBetween(1, 10)
                .WithMessage("Score should be between 1-10 to submit the rating.");
        }
    }
}
