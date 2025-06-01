namespace GHR.EmployeeManagement.Application.Queries.Search
{
    using FluentValidation;

    public class SearchEmployeesByNameQueryValidator : AbstractValidator<SearchEmployeesByNameQuery>
    {
        public SearchEmployeesByNameQueryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name query parameter is required.")
                .MinimumLength(2)
                .WithMessage("Name must be at least 2 characters long.");
        }
    } 
}
