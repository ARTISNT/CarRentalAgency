using CarService.Domain.Common;

namespace CarService.Domain.Cars.Enums;

public class EngineType : Enumeration
{
    public static readonly EngineType Gasoline = new EngineType(1, "Gasoline");
    public static readonly EngineType Diesel = new EngineType(2, "Diesel");
    public static readonly EngineType HybridGasoline = new EngineType(3, "HybridGasoline"); 
    public static readonly EngineType HybridDiesel = new EngineType(4, "HybridDiesel");
    public static readonly EngineType Electric = new EngineType(5, "Electric");
        
    public EngineType(int id, string name) : base(id, name)
    {
    }
}