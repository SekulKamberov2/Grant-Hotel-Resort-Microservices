namespace IdentityServer.Application.Commands.AssignRole
{
    using FluentValidation;
    public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
    {
        public AssignRoleCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User Id is required.")
                .GreaterThan(0).WithMessage("User Id must be a positive number.");

            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role Id is required.")
                .GreaterThan(0).WithMessage("Role Id must be a positive number.");
        }
    }
}
