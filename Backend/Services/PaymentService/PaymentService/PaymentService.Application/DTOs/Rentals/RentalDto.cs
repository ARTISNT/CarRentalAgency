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
        decimal DepositAmount
    );
}
