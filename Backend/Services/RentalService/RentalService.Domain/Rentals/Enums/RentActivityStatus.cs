using RentalService.Domain.Common;

namespace RentalService.Domain.Rentals.Enums;

public class RentActivityStatus : Enumeration
{
    public static readonly RentActivityStatus Active = new RentActivityStatus(1, "Active");
    public static readonly RentActivityStatus Completed = new RentActivityStatus(2, "Completed");
    public static readonly RentActivityStatus Cancelled = new RentActivityStatus(3, "Cancelled");
    public static readonly RentActivityStatus AwaitingConfirmation = new RentActivityStatus(4, "AwaitingConfirmation");
    
    public RentActivityStatus(int id, string name) : base(id, name)
    {
    }
    
    public static RentActivityStatus FromName(string name)
    {
        return GetAll<RentActivityStatus>()
                   .FirstOrDefault(x => x.Name == name)
               ?? throw new ArgumentException($"Unknown status: {name}");
    } 
}