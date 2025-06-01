namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesByFacility
{
    using FluentValidation;
    public class GetEmployeesByFacilityQueryValidator : AbstractValidator<GetEmployeesByFacilityQuery>
    {
        public GetEmployeesByFacilityQueryValidator()
        {
            RuleFor(x => x.FacilityId)
                .GreaterThan(0)
                .WithMessage("FacilityId must be greater than 0.");
        }
    } 
}
