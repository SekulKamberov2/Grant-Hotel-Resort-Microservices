namespace GHR.SharedKernel
{
    using Microsoft.Extensions.Logging;

    using GHR.SharedKernel.Exceptions;
    public static class ExecuteLogging
    {   
        public static async Task<IdentityResult<T>> ExecuteWithLogging<T>(
            Func<Task<T>> action,
            ILogger logger,
            string successMessage,
            string errorMessage,
            params object[] args)
        {
            try
            {
                var result = await action();
                if (EqualityComparer<T>.Default.Equals(result, default)) 
                    return IdentityResult<T>.Failure(errorMessage);
              
                logger.LogInformation(successMessage, args);
                return IdentityResult<T>.Success(result);
            }
            catch (RepositoryException ex)
            {
                logger.LogError(ex, errorMessage, args);
                return IdentityResult<T>.Failure(errorMessage);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred while executing action.");
                throw; // Rethrow unexpected exceptions
            }
        }
    }
}
