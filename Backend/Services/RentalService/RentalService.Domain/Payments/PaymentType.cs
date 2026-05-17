using RentalService.Domain.Common;

namespace RentalService.Domain.Payments;

public class PaymentType : Enumeration
{
    public static readonly PaymentType PrePayment = new PaymentType(1, "PrePayment");
    public static readonly PaymentType FinalPayment = new PaymentType(2, "FinalPayment");
    public static readonly PaymentType Deposit = new PaymentType(3, "Deposit");
    public static readonly PaymentType Fine = new PaymentType(4, "Fine");
    public static readonly PaymentType Refund = new PaymentType(5, "Refund");
    
    public PaymentType(int id, string name) : base(id, name)
    {
    }
}