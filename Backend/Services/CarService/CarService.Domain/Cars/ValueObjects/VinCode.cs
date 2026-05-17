using System.Text.RegularExpressions;
using CarService.Domain.Common;

namespace CarService.Domain.Cars.ValueObjects;

public record VinCode :  IValueObject
{
    private const int ValidLength = 17;
    private static readonly Regex VinRegex = new(@"^[A-HJ-NPR-Z0-9]{17}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; init; }

    public VinCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("VIN-code cannot be null or whitespace.");

        var normalized = value.Trim().ToUpper();

        if (normalized.Length != ValidLength)
            throw new ArgumentException($"VIN-code should contains exactly {ValidLength} characters.");

        if (!VinRegex.IsMatch(normalized))
            throw new ArgumentException("VIN-code contains unacceptable characters or invalid VIN-code format.");

        Value = normalized;
    }

    public string GetWmi() => Value.Substring(0, 3); 
    public string GetVds() => Value.Substring(3, 6);
    public string GetVis() => Value.Substring(9, 8); 
    public char GetModelYearCode() => Value[9];      
    
    public override string ToString() => Value;
}
