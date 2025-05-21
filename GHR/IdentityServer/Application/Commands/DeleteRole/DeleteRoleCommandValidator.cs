namespace IdentityServer.Application.Commands.DeleteRole
{
    using FluentValidation;
    public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
    {
        public DeleteRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role Id is required.")
                .GreaterThan(0).WithMessage("Role Id must be a positive number.");
        }
    }
}
 