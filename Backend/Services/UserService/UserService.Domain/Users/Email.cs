using System.Text.RegularExpressions;
using UserService.Domain.Common;

namespace UserService.Domain.Users;

public class Email : ValueObject
{
    public string Value { get; private set; }
    
    public const int MaxEmailLength = 30;
    public const int MinEmailLength = 6;
    
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    
    private Email() {} 

    public Email(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentNullException(nameof(email));
        
        if (!EmailRegex.IsMatch(email))
            throw new ArgumentException("Invalid email format.");
        
        if(email.Length is < MinEmailLength or > MaxEmailLength)
            throw new ArgumentException("Email must be between 6 and 30 characters long.");

        Value = email.Trim().ToLowerInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}