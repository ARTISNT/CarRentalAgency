namespace RentalService.Domain.Payments;

public interface IPaymentRepository
{
    public Task<Payment> GetPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);
    public Task<Payment> GetPaymentByRentIdAsync(Guid rentalId, CancellationToken cancellationToken = default);
    public Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default);
    public Task UpdatePaymentAsync(Payment payment, CancellationToken cancellationToken = default);
}