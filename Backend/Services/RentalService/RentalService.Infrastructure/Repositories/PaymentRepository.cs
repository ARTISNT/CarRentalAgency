using Microsoft.EntityFrameworkCore;
using RentalService.Domain.Payments;

namespace RentalService.Infrastructure.Repositories;

public class PaymentRepository(RentalServiceContext dbContext) : IPaymentRepository
{
    public async Task<Payment> GetPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
        return payment;
    }

    public async Task<Payment> GetPaymentByRentIdAsync(Guid rentalId, CancellationToken cancellationToken = default)
    {
        var payment = await dbContext.Payments.FirstOrDefaultAsync(p => p.RentalId == rentalId, cancellationToken);
        return payment;
    }

    public async Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await dbContext.Payments.AddAsync(payment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        dbContext.Payments.Update(payment);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}