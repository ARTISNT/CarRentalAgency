using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Status Status { get; private set; } = default!;
        public PaymentType PaymentType { get; private set; } = default!;
        public decimal Amount { get; private set; }
        public string ExternalToken { get; private set; } = default!;
        public Guid RentalId { get; private set; }
        public Guid PaymentId { get; private set; }
        public PaymentMethod? PaymentMethod { get; private set; }
        public bool IsRefunded { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime PaymentDate { get; private set; }

        private Transaction() { }

        public Transaction(decimal amount, string externalToken, Guid paymentId, Guid rentalId, PaymentType paymentType)
        {
            Status = Status.Pending;
            PaymentType = paymentType;
            Amount = amount;
            ExternalToken = externalToken;
            RentalId = rentalId;
            PaymentId = paymentId;
            IsRefunded = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void ConfirmSuccess()
        {
            if (Status == Status.Success) return;

            if (Status == Status.Failed)
                throw new InvalidOperationException("Cannot change status if it Failed");
            Status = Status.Success;
            PaymentDate = DateTime.UtcNow;
        }

        public void MarkRefunded()
        {
            if (PaymentType != PaymentType.Deposit)
                throw new InvalidOperationException("Only deposit transactions can be refunded");

            IsRefunded = true;
        }
    }
}
