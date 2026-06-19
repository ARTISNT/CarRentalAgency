namespace RentalService.Application.Abstractions;

public interface IPaymentServiceClient
{
    Task RefundDepositAsync(Guid rentalId);
}
