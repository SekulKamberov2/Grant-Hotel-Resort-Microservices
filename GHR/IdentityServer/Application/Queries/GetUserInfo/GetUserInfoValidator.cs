namespace IdentityServer.Application.Queries.GetUserInfo
{
    using FluentValidation;
    public class GetUserInfoValidator : AbstractValidator<GetUserInfoQuery>
    {
        public GetUserInfoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User Id is required.")
                .GreaterThan(0).WithMessage("User Id must be a positive number."); 
        }
    }
}
