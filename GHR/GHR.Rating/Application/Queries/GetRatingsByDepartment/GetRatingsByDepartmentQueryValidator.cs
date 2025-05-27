namespace GHR.Rating.Application.Queries
{
    using FluentValidation;
    public class GetRatingsByDepartmentQueryValidator : AbstractValidator<GetRatingsByDepartmentQuery>
    {
        public GetRatingsByDepartmentQueryValidator()
        {
            RuleFor(x => x.DepartmentId)
                .GreaterThan(0)
                .WithMessage("DepartmentId must be greater than zero.");
        }
    }
}
