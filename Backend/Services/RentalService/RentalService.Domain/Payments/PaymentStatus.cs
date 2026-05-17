using RentalService.Domain.Common;

namespace RentalService.Domain.Payments;

public class PaymentStatus : Enumeration
{
    public static readonly PaymentStatus Pending = new PaymentStatus(1, "Pending");
    public static readonly PaymentStatus PartiallyPaid = new PaymentStatus(2, "Partially paid");
    public static readonly PaymentStatus Paid = new PaymentStatus(3, "Paid");
    public static readonly PaymentStatus Refunded = new PaymentStatus(4, "Refunded");
    public static readonly PaymentStatus Failed = new PaymentStatus(5, "Failed");
        
    public PaymentStatus(int id, string name) : base(id, name)
    {
    }
}