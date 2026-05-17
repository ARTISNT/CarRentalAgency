using CarService.Domain.Common;

namespace CarService.Domain.Cars.ValueObjects;

public record CarModelInfo : IValueObject
{
    public string Model { get; init; }
    public string Brand { get; init; }
    public string? Generation { get; init; }
    public bool IsFacelift { get; init; }
    public string? Variant { get; init; }

    private const int MaxLength = 50;
    private const int MinLength = 3;
    private const int GenerationMinLength = 1;
    
    public CarModelInfo(string model, string brand, string? generation = null, string? variant = null, bool isFacelift = false)
    {
        ValidateString(model, nameof(model));
        ValidateString(brand, nameof(brand));
        
        if (generation != null) ValidateGeneration(generation);
        
        if (variant != null) ValidateString(variant, nameof(variant));

        Model = model;
        Brand = brand;;
        Generation = generation;
        Variant = variant;
        IsFacelift = isFacelift;
    }

    private static void ValidateString(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be empty", paramName);

        if (value.Length < MinLength || value.Length > MaxLength)
            throw new ArgumentOutOfRangeException(paramName, 
                $"{paramName} length must be between {MinLength} and {MaxLength}.");
    }

    private static void ValidateGeneration(string generation)
    {
        if(string.IsNullOrWhiteSpace(generation))
            throw new ArgumentException($"{nameof(generation)} cannot be empty", nameof(generation));
            
        if(generation.Length >  MaxLength || generation.Length < GenerationMinLength)
            throw new ArgumentOutOfRangeException(nameof(generation), 
                $"{generation} length must be between {MinLength} and {MaxLength}.");
    }
}