namespace GHR.HelpDesk.Repositories
{
    using GHR.SharedKernel.Exceptions;
    public static class RepositoryHelper
    {
        public static async Task<T> ExecuteWithHandlingAsync<T>(Func<Task<T>> operation, string errorMessage)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                throw new RepositoryException(errorMessage, ex);
            }
        }
    }
}
