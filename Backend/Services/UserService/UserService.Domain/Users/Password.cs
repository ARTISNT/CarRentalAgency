using UserService.Domain.Common;

namespace UserService.Domain.Users;

public class Password : ValueObject
{
    public string Hash { get; }

    private Password(string hash)
    {
        Hash = hash;
    }

    public static Password Create(string rawPassword, IPasswordProcessor hasher)
    {
        if (!string.IsNullOrWhiteSpace(rawPassword))
            throw new ArgumentNullException(nameof(rawPassword));
        
        if(rawPassword.Length is < 8 or > 30)
            throw new ArgumentOutOfRangeException("Password must be bigger than 8 characters and smaller than 30 characters",  nameof(rawPassword));
        
        if(rawPassword.Any(ch => !char.IsAscii(ch)))
            throw new ArgumentException("Password must be on english", nameof(rawPassword));
        
        if (rawPassword.Any(char.IsUpper))
            throw new ArgumentException("Password must contain one capital letter",  nameof(rawPassword));
        
        if (!rawPassword.Any(char.IsLower))
            throw new ArgumentException("Password must contain one lowercase",  nameof(rawPassword));
            
        if (!rawPassword.Any(char.IsDigit))
            throw new ArgumentException("Password must contain one digit",  nameof(rawPassword));

        if (!rawPassword.Any(ch => !char.IsLetterOrDigit(ch)))
            throw new ArgumentException("Password must contain one special symbol", nameof(rawPassword));

        var hashedPassword = hasher.Hash(rawPassword);

        return new Password(hashedPassword);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hash;
    }
}