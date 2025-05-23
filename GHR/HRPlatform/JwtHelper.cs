using System.Text;

namespace PeopleManagementAPI
{
    public static class JwtHelper
    {
        public static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static string Base64UrlEncode(string input)
        {
            return Base64UrlEncode(Encoding.UTF8.GetBytes(input));
        }
    }
}
