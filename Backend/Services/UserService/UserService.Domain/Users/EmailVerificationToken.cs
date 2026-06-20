using UserService.Domain.Common;

namespace UserService.Domain.Users;

public sealed class EmailVerificationToken : ValueObject
{
    public string TokenHash { get; }
    public DateTime ExpiresAt { get; }
    public DateTime CreatedAt { get; }

    private EmailVerificationToken(string tokenHash, DateTime expiresAt, DateTime createdAt)
    {
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
    }

    public static EmailVerificationToken Create(string tokenHash, DateTime expiresAt, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("Token hash must be provided.", nameof(tokenHash));

        if (expiresAt <= createdAt)
            throw new ArgumentException("ExpiresAt must be greater than CreatedAt.", nameof(expiresAt));

        return new EmailVerificationToken(tokenHash, expiresAt, createdAt);
    }

    public bool IsExpired(DateTime now) => now >= ExpiresAt;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TokenHash;
        yield return ExpiresAt;
        yield return CreatedAt;
    }
}
