using Microsoft.EntityFrameworkCore;
using RentalService.Domain.Payments;

namespace RentalService.Infrastructure.Repositories;

public class PaymentRepository(RentalServiceContext dbContext) : IPaymentRepository
{
    public async Task<Payment> GetPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await dbContext.Payments
            .Include(x => x.Transactions)
            .FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
        return payment;
    }

    public async Task<Payment> GetPaymentByRentIdAsync(Guid rentalId, CancellationToken cancellationToken = default)
    {
        var payment = await dbContext.Payments
            .Include(x => x.Transactions)
            .FirstOrDefaultAsync(p => p.RentalId == rentalId, cancellationToken);
        return payment;
    }

    public async Task<Dictionary<Guid, Payment>> GetPaymentsByRentIdsAsync(IEnumerable<Guid> rentalIds, CancellationToken cancellationToken = default)
    {
        return await dbContext.Payments
            .Include(x => x.Transactions)
            .Where(p => rentalIds.Contains(p.RentalId))
            .ToDictionaryAsync(p => p.RentalId, cancellationToken);
    }

    public async Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await dbContext.Payments.AddAsync(payment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}