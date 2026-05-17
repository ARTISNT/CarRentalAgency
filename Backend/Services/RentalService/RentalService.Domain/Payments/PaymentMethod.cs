using RentalService.Domain.Common;

namespace RentalService.Domain.Payments;

public class PaymentMethod : Enumeration
{
    
    public static readonly PaymentMethod Card = new PaymentMethod(1, "Card");
    public static readonly PaymentMethod Cash = new PaymentMethod(2, "Cash");
    public static readonly PaymentMethod BankTransfer = new PaymentMethod(3, "BankTransfer");
    public static readonly PaymentMethod System = new PaymentMethod(4, "System");
    
    public PaymentMethod(int id, string name) : base(id, name)
    {
    }
}