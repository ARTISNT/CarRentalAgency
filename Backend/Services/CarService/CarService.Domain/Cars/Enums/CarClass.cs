using CarService.Domain.Common;

namespace CarService.Domain.Cars.Enums;

public class CarClass : Enumeration
{
    public static readonly CarClass Economy = new CarClass(1, "Economy");
    public static readonly CarClass Standard = new CarClass(2, "Standard");
    public static readonly CarClass Business = new CarClass(3, "Business");
    public static readonly CarClass Premium = new CarClass(4, "Premium");
    
    private CarClass(int id, string name) : base(id, name)
    {
    }
}