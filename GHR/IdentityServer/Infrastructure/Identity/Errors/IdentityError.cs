namespace IdentityServer.Infrastructure.Identity.Errors
{
    public class IdentityError  //not using delete  
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public IdentityError(string code, string description)
        {
            Code = code;
            Description = description;
        }
         
        public static IdentityError CreateError(string code, string description)
        {
            return new IdentityError(code, description);
        }
    }
}
