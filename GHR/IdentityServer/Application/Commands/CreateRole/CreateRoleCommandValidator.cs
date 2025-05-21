namespace IdentityServer.Application.Commands.CreateRole
{
    using FluentValidation; 
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MinimumLength(2).WithMessage("Description must be at least 3 characters long.")
                .MaximumLength(200).WithMessage("Description must not exceed 200 characters.");
        }
    }
}
