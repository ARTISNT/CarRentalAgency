namespace PaymentService.Application.DTOs.Rentals
{
    public record RentalDto(
        Guid RentalId,
        string UserName,
        string UserPhone,
        string CarName,
        DateTime StartDate,
        DateTime EndDate,
        decimal TotalPrice,
        decimal DepositAmount,
        decimal PaidAmount,
        decimal RequiredAmount,
        decimal RemainingAmount,
        decimal FineOutstanding,
        decimal AdditionalOutstanding,
        string PaymentStatus,
        string ActivityStatus,
        DateTime? DepositPaidAt,
        List<PaymentTransactionDto> Transactions
    );

    public record PaymentTransactionDto(
        Guid Id,
        decimal Amount,
        string Type,
        string Method,
        string Status,
        string ExternalTransactionId,
        string? Description,
        DateTime CreatedAt,
        DateTime? CompletedAt,
        bool IsRefunded
    );
}
