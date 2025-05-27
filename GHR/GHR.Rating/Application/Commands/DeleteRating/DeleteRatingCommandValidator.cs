namespace GHR.Rating.Application.Commands.DeleteRating
{
    using FluentValidation;
    public class DeleteRatingCommandValidator : AbstractValidator<DeleteRatingCommand>
    {
        public DeleteRatingCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Rating Id must be greater than zero.");
        }
    }
}
