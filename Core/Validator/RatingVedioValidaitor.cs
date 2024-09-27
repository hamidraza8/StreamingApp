using Core.DTOs.rating_vedioDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validaitor
{
    public class RatingVedioValidator : AbstractValidator<RatingVedioDTO>
    {
        public RatingVedioValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.VideoId)
                .NotEmpty()
                .WithMessage("Video ID is required.");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");
        }
    }
}
