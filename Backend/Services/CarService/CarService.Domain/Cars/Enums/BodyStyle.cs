using CarService.Domain.Common;

namespace CarService.Domain.Cars.Enums;
public class BodyStyle : Enumeration
{
    public static readonly BodyStyle Sedan = new BodyStyle(1, "Sedan");
    public static readonly BodyStyle Hatchback = new BodyStyle(2, "Hatchback");
    public static readonly BodyStyle SUV = new BodyStyle(3, "SUV");
    public static readonly BodyStyle Crossover = new BodyStyle(4, "Crossover");
    public static readonly BodyStyle StationWagon = new BodyStyle(5, "Station Wagon");
    public static readonly BodyStyle Minivan = new BodyStyle(6, "Minivan");
    public static readonly BodyStyle Van = new BodyStyle(7, "Van"); 
    public static readonly BodyStyle Coupe = new BodyStyle(8, "Coupe");
    public static readonly BodyStyle Convertible = new BodyStyle(9, "Convertible"); 
    public static readonly BodyStyle Pickup = new BodyStyle(10, "Pickup");
    public static readonly BodyStyle Limousine = new BodyStyle(11, "Limousine");
    public static readonly BodyStyle Roadster = new BodyStyle(12, "Roadster");

    public BodyStyle(int id, string name) : base(id, name)
    {
    }
}
