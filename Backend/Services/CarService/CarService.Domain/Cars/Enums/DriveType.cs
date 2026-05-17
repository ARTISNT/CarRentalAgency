using CarService.Domain.Common;

namespace CarService.Domain.Cars.Enums;

public class DriveType : Enumeration
{
    public static readonly DriveType Fwd = new DriveType(1, "Fwd");
    public static readonly DriveType Rwd = new DriveType(2, "Rwd");
    public static readonly DriveType Awd = new DriveType(3, "Awd");
    public static readonly DriveType FourByFour = new DriveType(4, "FourByFour");
    
    
    public DriveType(int id, string name) : base(id, name)
    {
    }
}