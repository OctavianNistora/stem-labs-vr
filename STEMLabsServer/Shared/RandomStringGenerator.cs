using System.Security.Cryptography;

namespace STEMLabsServer.Shared;

public class RandomStringGenerator
{
    private const string Pool = "abcdefghijklmnopqrstuvwxyz0123456789";
        
    public static string Generate(int length)
    {
        var chars = Enumerable.Range(0, length)
            .Select(_ => Pool[RandomNumberGenerator.GetInt32(0, Pool.Length)]);
        return new string(chars.ToArray());
    }
}