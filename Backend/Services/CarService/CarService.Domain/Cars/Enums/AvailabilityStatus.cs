using CarService.Domain.Common;

namespace CarService.Domain.Cars.Enums;
public class AvailabilityStatus : Enumeration
{
    public static readonly AvailabilityStatus Available = new(1, "Available");
    public static readonly AvailabilityStatus Rented = new(2, "Rented");
    public static readonly AvailabilityStatus Maintenance = new(3, "Maintenance");
    public static readonly AvailabilityStatus Broken = new(4, "Broken");

    private AvailabilityStatus(int id, string name) : base(id, name) { }
}
