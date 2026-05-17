using System.Text.RegularExpressions;
using UserService.Domain.Common;

namespace UserService.Domain.Users;

public class IdentityNumber : ValueObject
{
    public string Value { get; private set; }
    
    private static readonly Regex IdentityNumberRegex = 
        new Regex(@"^(?:[1-6](0[1-9]|[12][0-9]|3[01])(0[1-9]|1[0-2])\d{2}[ABCHEKM]\d{3}(РВ|ВА|BI)\d|\d{7}[A-Z]\d{3}[A-Z]{2}\d)$", 
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
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