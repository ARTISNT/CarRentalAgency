using System.Text.RegularExpressions;
using CarService.Domain.Common;

namespace CarService.Domain.Cars.ValueObjects;

public record LicensePlate :  IValueObject
{
    private static readonly Regex PlateRegex = new(@"^\d{4} [A-Z]{2}-[1-7]$");

    public string Value { get; init; }

    public LicensePlate (string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("License plate cannot be null or empty");

        var normalized = value.Trim().ToUpper();

        if (!PlateRegex.IsMatch(normalized))
            throw new ArgumentException("Incorrect format of license plates in republic of Belarus(valid format: '0000 AA-0').");

        Value = normalized;
    }
}