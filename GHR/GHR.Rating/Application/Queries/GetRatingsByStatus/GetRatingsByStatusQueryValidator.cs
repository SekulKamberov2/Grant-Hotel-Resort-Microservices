namespace GHR.Rating.Application.Queries.GetRatingsByStatus
{
    using FluentValidation;
    public class GetRatingsByStatusQueryValidator : AbstractValidator<GetRatingsByStatusQuery>
    {
        public GetRatingsByStatusQueryValidator()
        {
            RuleFor(x => x)
                .Must(x => x.IsApproved.HasValue || x.IsFlagged.HasValue || x.IsDeleted.HasValue)
                .WithMessage("At least one filter (IsApproved, IsFlagged, or IsDeleted) must be specified.");
        }
    }
}
