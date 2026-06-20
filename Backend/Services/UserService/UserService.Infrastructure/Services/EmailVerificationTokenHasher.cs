using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using UserService.Domain.Users;

namespace UserService.Infrastructure.Services;

public class EmailVerificationTokenHasher : IEmailVerificationTokenHasher
{
    private readonly byte[] _secret;

    public EmailVerificationTokenHasher(IConfiguration configuration)
    {
        var secret = configuration["EmailVerification:TokenSecret"]
            ?? throw new InvalidOperationException("EmailVerification:TokenSecret is not configured.");

        if (Encoding.UTF8.GetByteCount(secret) < 32)
            throw new InvalidOperationException("EmailVerification:TokenSecret must be at least 32 bytes.");

        _secret = Encoding.UTF8.GetBytes(secret);
    }

    public string Hash(string rawToken)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
            throw new ArgumentException("Token must be provided.", nameof(rawToken));

        using var hmac = new HMACSHA256(_secret);
        var bytes = Encoding.UTF8.GetBytes(rawToken);
        var hash = hmac.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public bool Verify(string rawToken, string tokenHash)
    {
        if (string.IsNullOrWhiteSpace(rawToken) || string.IsNullOrWhiteSpace(tokenHash))
            return false;

        var computed = Hash(rawToken);
        var a = Encoding.UTF8.GetBytes(computed);
        var b = Encoding.UTF8.GetBytes(tokenHash);
        return CryptographicOperations.FixedTimeEquals(a, b);
    }
}
