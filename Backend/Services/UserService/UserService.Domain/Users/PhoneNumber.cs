using System.Text.RegularExpressions;
using UserService.Domain.Common;

namespace UserService.Domain.Users;

public class PhoneNumber : ValueObject
{
    public string Value { get; private set; }
    
    private static readonly Regex PhoneRegex =
        new(@"^[+]?\d{10,15}$", RegexOptions.Compiled);

    private PhoneNumber(){}
    public PhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("PhoneNumber cannot be null.");
        
        if(!PhoneRegex.IsMatch(phoneNumber))
            throw new ArgumentException("Invalid phone number.");
        
        Value = phoneNumber.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return  Value;
    }
}