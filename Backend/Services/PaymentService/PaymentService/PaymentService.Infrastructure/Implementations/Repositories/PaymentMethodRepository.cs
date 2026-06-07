using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Abstractions.Repositories;
using PaymentService.Domain.Entities;
using PaymentService.Infrastructure.Persistence.DB;

namespace PaymentService.Infrastructure.Implementations.Repositories
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly PaymentContext _context;

        public PaymentMethodRepository(PaymentContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(paymentMethod, cancellationToken);
        }

        public async Task DeleteAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
        {
            _context.Remove(paymentMethod);
        }

        public async Task<IEnumerable<PaymentMethod>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var paymentMethods = await _context.PaymentMethods.ToListAsync(cancellationToken);
            return paymentMethods;
        }

        public async Task UpdateAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
        {
            _context.PaymentMethods.Update(paymentMethod);
        }
    }
}
