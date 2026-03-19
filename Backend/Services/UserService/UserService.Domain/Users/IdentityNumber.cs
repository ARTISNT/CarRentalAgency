using System.Text.RegularExpressions;
using UserService.Domain.Common;

namespace UserService.Domain.Users;

public class IdentityNumber : ValueObject
{
    public string Value { get; private set; }
    
    private static readonly Regex IdentityNumberRegex = 
        new Regex("^[1-6]\\d{6}[ABCKEMH]\\d{3}(PB|BA|BI)\\d$",  RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    private IdentityNumber(){}

    public IdentityNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentNullException(nameof(number));
      
        if(!IdentityNumberRegex.IsMatch(number))
            throw new ArgumentException("Invalid identity number");
      
        Value = number.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}