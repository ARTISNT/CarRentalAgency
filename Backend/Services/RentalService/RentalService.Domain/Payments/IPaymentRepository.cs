namespace RentalService.Domain.Payments;

public interface IPaymentRepository
{
    public Task<Payment> GetPaymentAsync(Guid paymentId);
    public Task<Payment> GetPaymentByRentIdAsync(Guid rentalId);
    public Task AddPaymentAsync(Payment payment);
    public Task UpdatePaymentAsync(Payment payment);
}