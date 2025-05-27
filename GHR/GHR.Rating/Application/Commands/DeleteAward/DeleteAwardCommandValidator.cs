namespace GHR.Rating.Application.Commands.DeleteAward
{
    using FluentValidation;
    public class DeleteAwardCommandValidator : AbstractValidator<DeleteAwardCommand>
    {
        public DeleteAwardCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Award ID must be greater than zero.");
        }
    }
}
