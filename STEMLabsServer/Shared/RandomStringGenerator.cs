using System.Security.Cryptography;

namespace STEMLabsServer.Shared;

// Generates a random string of a specified length using lowercase letters, uppercase letters, and digits.
public class RandomStringGenerator
{
    private const string Pool = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
    public static string Generate(int length)
    {
        var chars = Enumerable.Range(0, length)
            .Select(_ => Pool[RandomNumberGenerator.GetInt32(0, Pool.Length)]);
        return new string(chars.ToArray());
    }
}