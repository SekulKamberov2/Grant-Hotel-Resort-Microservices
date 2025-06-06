namespace GHR.SharedKernel.Helpers
{
    using Microsoft.Data.SqlClient;

    using GHR.SharedKernel.Exceptions;
    public static class RepositoryHelper
    {
        public static async Task<T> ExecuteWithHandlingAsync<T>(Func<Task<T>> operation, string errorMessage)
        {
            try
            {
                return await operation();
            }
            catch (SqlException ex)
            { 
                throw new RepositoryException($"{errorMessage} A SQL error occurred: {ex.Message}", ex);
            }
            catch (InvalidOperationException ex)
            {
                // common in Dapper when queries return unexpected results
                throw new RepositoryException($"{errorMessage} Invalid operation: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Fallback for unknown issues
                throw new RepositoryException($"{errorMessage} An unexpected error occurred.", ex);
            }
        }
    }
}
