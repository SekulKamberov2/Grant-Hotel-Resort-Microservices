using IdentityServer.Application.Interfaces; 
using System.Security.Cryptography;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int KeySize = 32;  // 256 bits
    private const int Iterations = 10000;
    private const char Delimiter = ';';

    public string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[SaltSize];
        rng.GetBytes(salt);   

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] key = pbkdf2.GetBytes(KeySize);   
         
        string hashedPassword = Convert.ToBase64String(salt) + Delimiter + Convert.ToBase64String(key);
        return hashedPassword;
    } 

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    { 
        var parts = hashedPassword.Split(Delimiter);
        if (parts.Length != 2)
            return false;  

        byte[] salt = Convert.FromBase64String(parts[0]);   
        byte[] storedKey = Convert.FromBase64String(parts[1]);  

        using var pbkdf2 = new Rfc2898DeriveBytes(providedPassword, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] providedKey = pbkdf2.GetBytes(KeySize);  
         
        return CryptographicOperations.FixedTimeEquals(providedKey, storedKey);
    }

}
