using RentalService.Domain.Common;

namespace RentalService.Domain.Payments;
public class PaymentTransaction : Entity
{
    public Money Amount { get; private set; }

    public PaymentType Type { get; private set; }

    public PaymentMethod Method { get; private set; }

    public string ExternalTransactionId { get; private set; }

    public TransactionStatus Status { get; private set; }

    public string? FailureReason { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    protected PaymentTransaction() {}

    public PaymentTransaction(
        Money amount,
        PaymentType type,
        PaymentMethod method,
        string externalTransactionId)
    {
        Amount = amount;
        Type = type;
        Method = method;

        ExternalTransactionId = externalTransactionId;

        Status = TransactionStatus.Pending;

        CreatedAtUtc = DateTime.UtcNow;
    }

    public void MarkCompleted()
    {
        if (Status == TransactionStatus.Completed)
            throw new InvalidOperationException(
                "Already completed");

        Status = TransactionStatus.Completed;

        CompletedAtUtc = DateTime.UtcNow;
    }

    public void MarkFailed(string reason)
    {
        if (Status == TransactionStatus.Completed)
            throw new InvalidOperationException(
                "Completed transaction cannot fail");

        FailureReason = reason;

        Status = TransactionStatus.Failed;
    }
}