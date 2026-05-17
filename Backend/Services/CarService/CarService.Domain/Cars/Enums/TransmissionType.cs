using CarService.Domain.Common;

namespace CarService.Domain.Cars.Enums;

public class TransmissionType : Enumeration
{
    public static readonly TransmissionType Manual = new TransmissionType(1, "Manual");
    public static readonly TransmissionType Automatic = new TransmissionType(2, "Automatic");
    public static readonly TransmissionType Variator = new TransmissionType(3, "Variator");
    public static readonly TransmissionType Robotic = new TransmissionType(4, "Robotic");
    
    public TransmissionType(int id, string name) : base(id, name)
    {
    }
}