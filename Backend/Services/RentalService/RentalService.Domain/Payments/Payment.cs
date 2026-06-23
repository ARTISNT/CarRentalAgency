using RentalService.Domain.Common;

namespace RentalService.Domain.Payments;

public class Payment : Entity, IAggregateRoot
{
    private readonly List<PaymentTransaction> _transactions = [];

    public Guid RentalId { get; private set; }
    public Money EstimatedAmount { get; private set; }
    public Money? FinalAmount { get; private set; }
    public Money DepositAmount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public Money Overpayment =>
        PaidAmount.Amount > RequiredAmount.Amount
            ? PaidAmount - RequiredAmount
            : Money.Zero(RequiredAmount.Currency);

    public Money Underpayment =>
        RequiredAmount.Amount > PaidAmount.Amount
            ? RequiredAmount - PaidAmount
            : Money.Zero(RequiredAmount.Currency); 
    
    public IReadOnlyCollection<PaymentTransaction> Transactions =>
        _transactions.AsReadOnly();

    protected Payment() {}

    public Payment(
        Guid rentalId,
        Money estimatedAmount,
        Money depositAmount)
    {
        if (estimatedAmount.Amount <= 0)
            throw new ArgumentException(
                "Estimated amount invalid");
        
        RentalId = rentalId;

        EstimatedAmount = estimatedAmount;
        DepositAmount = depositAmount;

        CreatedAtUtc = DateTime.UtcNow;

        Status = PaymentStatus.Pending; 
        
        // AddDomainEvent(
        //     new PaymentCreatedDomainEvent(
        //         Id,
        //         RentalId,
        //         DateTime.UtcNow));
    }

    public Money PaidAmount =>
        new(
            _transactions
                .Where(x => x.Status == TransactionStatus.Completed)
                .Sum(x =>
                    x.Type == PaymentType.Refund
                        ? -x.Amount.Amount
                        : x.Amount.Amount),
            EstimatedAmount.Currency);

    public Money RequiredAmount =>
        FinalAmount ?? EstimatedAmount;

    public Money RemainingAmount =>
        new(
            Math.Max(0, RequiredAmount.Amount - PaidAmount.Amount),
            RequiredAmount.Currency);

    public bool IsFullyPaid =>
        RemainingAmount.Amount <= 0;

    public Money FineOutstanding =>
        new(
            _transactions
                .Where(x => x.Type == PaymentType.Fine
                    && x.Status != TransactionStatus.Failed
                    && x.Status != TransactionStatus.Completed
                    && x.ExternalTransactionId.StartsWith("fine-"))
                .Sum(x => x.Amount.Amount),
            EstimatedAmount.Currency);

    public Money AdditionalOutstanding =>
        new(
            _transactions
                .Where(x => x.Type == PaymentType.Additional
                    && x.Status != TransactionStatus.Failed
                    && x.Status != TransactionStatus.Completed
                    && x.ExternalTransactionId.StartsWith("renewal-"))
                .Sum(x => x.Amount.Amount),
            EstimatedAmount.Currency);

    public bool HasOutstandingFines =>
        FineOutstanding.Amount > 0;

    public void FinalizeAmount(Money finalAmount)
    {
        if (finalAmount.Amount <= 0)
            throw new ArgumentException(
                "Final amount invalid");
        
        EnsureSameCurrency(finalAmount);

        FinalAmount = finalAmount;
        
        RecalculateStatus();

        // AddDomainEvent(
        //     new PaymentFinalizedDomainEvent(
        //         Id,
        //         RentalId,
        //         finalAmount.Amount,
        //         DateTime.UtcNow));
    }
    
    public void UpdateEstimatedAmount(Money newAmount)
    {
        if (newAmount.Amount <= 0)
            throw new  ArgumentException("Estimated amount invalid");

        EnsureSameCurrency(newAmount);

        EstimatedAmount = newAmount;

        RecalculateStatus();
    } 

    public Guid AddTransaction(
        Money amount,
        PaymentType type,
        PaymentMethod method,
        string externalTransactionId)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException(
                "Amount invalid");

        EnsureSameCurrency(amount);

        if (string.IsNullOrWhiteSpace(externalTransactionId))
            throw new ArgumentException(
                "Transaction id required");

        var transaction = new PaymentTransaction(
            amount,
            type,
            method,
            externalTransactionId);

        _transactions.Add(transaction);

        RecalculateStatus();

        // AddDomainEvent(
        //     new PaymentTransactionAddedDomainEvent(
        //         Id,
        //         transaction.Id,
        //         RentalId,
        //         DateTime.UtcNow));
        
        return transaction.Id;
    }

    public void CompleteTransaction(Guid transactionId)
    {
        var transaction = GetTransaction(transactionId);

        transaction.MarkCompleted();

        RecalculateStatus();

        // AddDomainEvent(
        //     new PaymentTransactionCompletedDomainEvent(
        //         Id,
        //         transactionId,
        //         RentalId,
        //         DateTime.UtcNow));
    }

    public void FailTransaction(
        Guid transactionId,
        string reason)
    {
        var transaction = GetTransaction(transactionId);

        transaction.MarkFailed(reason);

        RecalculateStatus();

        // AddDomainEvent(
        //     new PaymentTransactionFailedDomainEvent(
        //         Id,
        //         transactionId,
        //         RentalId,
        //         reason,
        //         DateTime.UtcNow));
    }

    public void Refund(
        Money amount,
        string? reason)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException(
                "Refund amount invalid");

        EnsureSameCurrency(amount);

        if (amount.Amount > PaidAmount.Amount)
            throw new InvalidOperationException(
                "Refund exceeds paid amount");

        var refundTransaction = new PaymentTransaction(
            amount,
            PaymentType.Refund,
            PaymentMethod.System,
            Guid.NewGuid().ToString());

        refundTransaction.MarkCompleted();

        _transactions.Add(refundTransaction);

        RecalculateStatus();

        // AddDomainEvent(
        //     new PaymentRefundedDomainEvent(
        //         Id,
        //         RentalId,
        //         amount.Amount,
        //         reason,
        //         DateTime.UtcNow));
    }

    public Guid AddFine(
        Money amount,
        string reason,
        Guid rentalId)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("Fine amount must be positive");

        if (rentalId == Guid.Empty)
            throw new ArgumentException("Rental id required");

        EnsureSameCurrency(amount);

        var baseCost = FinalAmount ?? EstimatedAmount;
        var newFinalAmount = new Money(
            baseCost.Amount + amount.Amount,
            baseCost.Currency);

        if (FinalAmount is null)
        {
            FinalizeAmount(newFinalAmount);
        }
        else
        {
            FinalAmount = newFinalAmount;
            RecalculateStatus();
        }

        var transaction = new PaymentTransaction(
            amount,
            PaymentType.Fine,
            PaymentMethod.System,
            $"fine-pending-{rentalId:D}",
            reason);

        _transactions.Add(transaction);

        RecalculateStatus();

        return transaction.Id;
    }

    public Guid AddAdditional(
        Money amount,
        string reason)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("Additional amount must be positive");

        EnsureSameCurrency(amount);

        var baseCost = FinalAmount ?? EstimatedAmount;
        var newFinalAmount = new Money(
            baseCost.Amount + amount.Amount,
            baseCost.Currency);

        if (FinalAmount is null)
        {
            FinalizeAmount(newFinalAmount);
        }
        else
        {
            FinalAmount = newFinalAmount;
            RecalculateStatus();
        }

        // Не создаём phantom-транзакцию здесь. Реальная запись о платеже появится
        // в DepositPaidConsumer, когда payment-service пришлёт DepositPaidIntegrationEvent
        // с TransactionId. Дополнительно сохраняем системную транзакцию "теневого" учёта,
        // которая будет закрыта по тому же TransactionId из шины.
        var shadowTransaction = new PaymentTransaction(
            amount,
            PaymentType.Additional,
            PaymentMethod.System,
            $"shadow-{Guid.NewGuid()}",
            reason);

        _transactions.Add(shadowTransaction);

        RecalculateStatus();

        return shadowTransaction.Id;
    }

    public void MarkTransactionCompleted(
        string externalTransactionId,
        DateTime completedAtUtc)
    {
        var transaction = _transactions
            .FirstOrDefault(x => x.ExternalTransactionId == externalTransactionId
                && x.Status != TransactionStatus.Completed);

        if (transaction is null)
            return;

        if (transaction.Status == TransactionStatus.Failed)
            return;

        transaction.MarkCompleted();
        RecalculateStatus();
    }

    private PaymentTransaction GetTransaction(Guid transactionId)
    {
        var transaction = _transactions
            .FirstOrDefault(x => x.Id == transactionId);

        if (transaction is null)
            throw new InvalidOperationException(
                "Transaction not found");

        return transaction;
    }

    private void RecalculateStatus()
    {
        var paidAmount = PaidAmount.Amount;

        if (paidAmount <= 0)
        {
            Status = PaymentStatus.Pending;
            return;
        }

        if (paidAmount < RequiredAmount.Amount)
        {
            Status = PaymentStatus.PartiallyPaid;
            return;
        }

        if (paidAmount >= RequiredAmount.Amount)
        {
            Status = PaymentStatus.Paid;
        }
    }

    private void EnsureSameCurrency(Money money)
    {
        if (money.Currency != EstimatedAmount.Currency)
            throw new InvalidOperationException(
                "Currency mismatch");
    }
}