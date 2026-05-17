using Microsoft.EntityFrameworkCore;
using RentalService.Domain.Payments;

namespace RentalService.Infrastructure.Repositories;

public class PaymentRepository(RentalServiceContext dbContext) : IPaymentRepository
{
    public async Task<Payment> GetPaymentAsync(Guid paymentId)
    {
        var payment = await dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);
        return payment;
    }

    public async Task<Payment> GetPaymentByRentIdAsync(Guid rentalId)
    {
        var payment = await dbContext.Payments.FirstOrDefaultAsync(p => p.RentalId == rentalId);
        return payment;
    }

    public async Task AddPaymentAsync(Payment payment)
    {
        await dbContext.Payments.AddAsync(payment);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdatePaymentAsync(Payment payment)
    {
        dbContext.Payments.Update(payment);
        await dbContext.SaveChangesAsync();
    }
}