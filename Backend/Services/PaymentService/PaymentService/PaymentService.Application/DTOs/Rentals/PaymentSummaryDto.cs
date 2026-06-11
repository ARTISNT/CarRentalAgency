namespace PaymentService.Application.DTOs.Rentals
{
    public record PaymentSummaryDto(
        Guid RentalId,
        decimal TotalPrice,
        decimal DepositAmount,
        decimal PaidAmount,
        decimal RequiredAmount,
        decimal RemainingAmount,
        decimal FineOutstanding,
        decimal AdditionalOutstanding,
        string PaymentStatus,
        List<PaymentTransactionDto> Transactions
    );
}
