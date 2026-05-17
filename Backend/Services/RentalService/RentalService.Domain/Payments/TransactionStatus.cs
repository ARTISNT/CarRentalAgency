using RentalService.Domain.Common;

namespace RentalService.Domain.Payments;

public class TransactionStatus : Enumeration
{
    public static readonly TransactionStatus Pending = new TransactionStatus(1, "Pending");
    public static readonly TransactionStatus Completed = new TransactionStatus(2, "Completed");
    public static readonly TransactionStatus Failed = new TransactionStatus(3, "Failed");
    
    public TransactionStatus(int id, string name) : base(id, name)
    {
        
    }
}