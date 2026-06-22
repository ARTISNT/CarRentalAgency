namespace RentalService.Application.Exceptions;

public class UnpaidFineException : Exception
{
    public decimal OutstandingAmount { get; }

    public UnpaidFineException(string message, decimal outstandingAmount = 0m)
        : base(message)
    {
        OutstandingAmount = outstandingAmount;
    }
}
