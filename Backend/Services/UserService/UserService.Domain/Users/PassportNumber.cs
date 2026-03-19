using System.Text.RegularExpressions;
using UserService.Domain.Common;

namespace UserService.Domain.Users;

public class PassportNumber : ValueObject
{
    public string Value { get; private set; }
    
    private static readonly Regex PassportNumberRegex = 
        new Regex("^[A-Za-z]{2}[0-9]{7}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    private PassportNumber(){}

    public PassportNumber(string number)
    {
        if(string.IsNullOrWhiteSpace(number))
            throw new ArgumentNullException(nameof(number));
      
        if (!PassportNumberRegex.IsMatch(number))
            throw new ArgumentException("Invalid passport number");
         
        Value = number.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}