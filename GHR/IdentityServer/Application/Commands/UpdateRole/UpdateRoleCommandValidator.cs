using FluentValidation;
using IdentityServer.Application.Commands.UpdateRole;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID is required.")
            .GreaterThan(0).WithMessage("ID must be a positive number.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.") // This catches empty or null strings
            .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(3).WithMessage("Description must be at least 3 characters long.")
            .MaximumLength(200).WithMessage("Description must not exceed 200 characters.");
    }
}
