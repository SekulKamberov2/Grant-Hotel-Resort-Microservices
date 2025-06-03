namespace GHR.SharedKernel
{
    using Microsoft.Extensions.Logging; 
    using GHR.SharedKernel.Exceptions;
    public static class ExecuteLogging
    {   
        public static async Task<Result<T>> ExecuteWithLogging<T>(
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
                    return Result<T>.Failure(errorMessage);
              
                logger.LogInformation(successMessage, args);
                return Result<T>.Success(result);
            }
            catch (RepositoryException ex)
            {
                logger.LogError(ex, errorMessage, args);
                return Result<T>.Failure(errorMessage);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred while executing action.");
                throw; 
            }
        }
    }
}
