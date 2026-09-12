using System.Security.Cryptography;
using System.Text;

namespace Pulse.Services;

public static class PasswordHasher
{
    /// <summary>Kthen hash-in SHA-256 të fjalëkalimit si varg hex me shkronja të vogla.</summary>
    public static string Hash(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static bool Verify(string password, string hash) =>
        string.Equals(Hash(password), hash, StringComparison.OrdinalIgnoreCase);
}
