namespace GHR.Rating.Application.Queries.GetAverageRatin
{
    using FluentValidation;
    using GHR.Rating.Application.Queries; 
    public class GetAverageRatingQueryValidator : AbstractValidator<GetAverageRatingQuery>
    {
        public GetAverageRatingQueryValidator()
        {
            RuleFor(x => x.DepartmentId)
                .GreaterThan(0)
                .WithMessage("DepartmentId must be greater than zero.");
        }
    }

}
