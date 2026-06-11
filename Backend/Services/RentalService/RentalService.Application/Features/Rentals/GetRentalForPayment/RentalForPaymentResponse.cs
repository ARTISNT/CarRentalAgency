namespace RentalService.Application.Features.Rentals.GetRentalForPayment;
public class RentalForPaymentResponse
{
    public Guid RentalId { get; set; }
    public string UserName { get; set; } = default!;
    public string UserPhone { get; set; } = default!;
    public string CarName { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RequiredAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal FineOutstanding { get; set; }
    public decimal AdditionalOutstanding { get; set; }
    public string PaymentStatus { get; set; } = default!;
    public string ActivityStatus { get; set; } = default!;
    public DateTime? DepositPaidAt { get; set; }
    public List<PaymentTransactionDto> Transactions { get; set; } = new();
}

public class PaymentTransactionDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = default!;
    public string Method { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string ExternalTransactionId { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
