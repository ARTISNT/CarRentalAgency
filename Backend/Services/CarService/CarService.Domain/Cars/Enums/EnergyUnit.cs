using CarService.Domain.Common;

namespace CarService.Domain.Cars.Enums;

public class EnergyUnit : Enumeration
{
     public static readonly EnergyUnit Liters = new EnergyUnit(1,"Liters");
     public static readonly EnergyUnit KiloWattHours=  new EnergyUnit(2,"KiloWattHours");
    
    public EnergyUnit(int id, string name) : base(id, name)
    {
    }
}