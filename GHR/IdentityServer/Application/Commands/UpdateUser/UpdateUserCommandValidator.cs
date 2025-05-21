namespace IdentityServer.Application.Commands.UpdateUser
{
    using FluentValidation;
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID is required.")
                .GreaterThan(0).WithMessage("ID must be a positive number.");

            RuleFor(x => x.Email)
                .MinimumLength(5).WithMessage("Email must be at least 5 characters long.")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Email format is invalid.")   
                .When(x => !string.IsNullOrEmpty(x.Email));  
                  
            RuleFor(x => x.PhoneNumber) 
                .MinimumLength(10).WithMessage("Phone number must be at least 10 characters long.")
                .MaximumLength(50).WithMessage("Phone number must not exceed 50 characters.")
                .Matches(@"^[\+\d\-\(\)\s]+$").WithMessage("Phone number can only contain digits, spaces, '+', '(', ')', or '-' characters.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
        }
    }  
}
