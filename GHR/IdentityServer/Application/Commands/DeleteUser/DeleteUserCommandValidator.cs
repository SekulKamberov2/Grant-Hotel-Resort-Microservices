namespace IdentityServer.Application.Commands.DeleteUser
{
    using FluentValidation;

    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User Id is required.")
                .GreaterThan(0).WithMessage("User Id must be a positive number.");
        }
    }
}
