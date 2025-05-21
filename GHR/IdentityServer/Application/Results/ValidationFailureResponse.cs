namespace IdentityServer.Application.Results
{
    public class ValidationFailureResponse
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
        public object AttemptedValue { get; set; }
    }

}
