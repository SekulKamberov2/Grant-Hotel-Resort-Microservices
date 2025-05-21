namespace IdentityServer.Domain.Models
{
    public class Jwt
    {
        public string Header { get; set; }
        public string Payload { get; set; }
        public string Signature { get; set; }

        public Jwt(string header, string payload, string signature)
        {
            Header = header;
            Payload = payload;
            Signature = signature;
        }

        public string GetToken()
        {
            return $"{Header}.{Payload}.{Signature}";
        }
    }
}
