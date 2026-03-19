using System.Text.RegularExpressions;
using UserService.Domain.Common;

namespace UserService.Domain.Users;

public class Email : ValueObject
{
    public string Value { get; private set; }
    
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    
    private Email() {} 

    public Email(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentNullException(nameof(email));
        
        if (!EmailRegex.IsMatch(email))
            throw new ArgumentException("Invalid email format.");

        Value = email.Trim().ToLowerInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}