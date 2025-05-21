namespace IdentityServer.Application.Behaviors
{
    using MediatR;
    using FluentValidation;

    using IdentityServer.Application.Results; 
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators; 
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                var error = string.Join("; ", failures.Select(f => f.ErrorMessage));
                var errorResultType = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(IdentityResult<>)
                    .MakeGenericType(errorResultType)
                    .GetMethod("Failure");

                return (TResponse)failureMethod.Invoke(null, new object[] { error })!;
            } 
            return await next();
        }
    } 
}
