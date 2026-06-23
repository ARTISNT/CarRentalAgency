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
        public string? TrackingId { get; private set; }
        public Guid RentalId { get; private set; }
        public Guid PaymentId { get; private set; }
        public PaymentMethod? PaymentMethod { get; private set; }
        public bool IsRefunded { get; private set; }
        public string? Description { get; private set; }
        public string? ExternalReceiptUrl { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime PaymentDate { get; private set; }

        private Transaction() { }

        public Transaction(decimal amount, string externalToken, Guid paymentId, Guid rentalId, PaymentType paymentType, string? description = null, string? trackingId = null)
        {
            Status = Status.Pending;
            PaymentType = paymentType;
            Amount = amount;
            ExternalToken = externalToken;
            TrackingId = trackingId;
            RentalId = rentalId;
            PaymentId = paymentId;
            IsRefunded = false;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }

        public void ConfirmSuccess(string? receiptUrl = null)
        {
            if (Status == Status.Success) return;

            if (Status == Status.Failed)
                throw new InvalidOperationException("Cannot change status if it Failed");
            Status = Status.Success;
            PaymentDate = DateTime.UtcNow;
            ExternalReceiptUrl = receiptUrl;
        }

        public void MarkRefunded()
        {
            if (PaymentType != PaymentType.Deposit
                && PaymentType != PaymentType.Fine
                && PaymentType != PaymentType.Additional)
            {
                throw new InvalidOperationException("Only deposit, fine, or additional transactions can be refunded");
            }

            IsRefunded = true;
        }

        public void AttachReceipt(string receiptUrl)
        {
            ExternalReceiptUrl = receiptUrl;
        }
    }
}
