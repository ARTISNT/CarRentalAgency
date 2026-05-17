using CarService.Domain.Common;

namespace CarService.Domain.Cars.ValueObjects;

public record PricePerHour :  IValueObject
{
    public double Price { get; init; }
    
    public PricePerHour(double price)
    {
        if(price <= 0)
            throw new ArgumentException("Price must be greater than zero");
        
        Price = price;
    }
} 