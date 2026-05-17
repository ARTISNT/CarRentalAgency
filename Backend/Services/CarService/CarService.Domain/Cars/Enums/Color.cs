namespace CarService.Domain.Cars.Enums;

public record Color 
{
    public string Value { get; }

    public Color(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Color must not be empty", nameof(value));
        
        if(value.Any(c => char.IsDigit(c)))
            throw new ArgumentException("Color must not contain digits", nameof(value));
        
        if(value.Length is < 3 or > 50)
            throw new ArgumentException("Color must not contain more than 50 characters and less then 3", nameof(value));
        
        Value = value.Trim();
    }
}