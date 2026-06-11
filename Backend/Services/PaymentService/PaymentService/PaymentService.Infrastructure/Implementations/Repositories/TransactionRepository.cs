using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Abstractions.Repositories;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;
using PaymentService.Infrastructure.Persistence.DB;

namespace PaymentService.Infrastructure.Implementations.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly PaymentContext _paymentContext;

        public TransactionRepository(PaymentContext paymentContext)
        {
            _paymentContext = paymentContext;
        }

        public async Task CreateAsync(Transaction transaction)
        {
            await _paymentContext.Transactions
                .AddAsync(transaction);
        }
        public async Task<Transaction?> GetByIdAsync(Guid id)
        {
            var transaction = await _paymentContext.Transactions
                .FirstOrDefaultAsync(t => t.Id == id);
            return transaction;
        }
        public async Task<Transaction?> GetByRentalIdAsync(Guid rentalId)
        {
            var transaction = await _paymentContext.Transactions
                .Where(t => t.RentalId == rentalId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
            return transaction;
        }
        public async Task<Transaction?> GetByRentalIdAndTypeAsync(Guid rentalId, PaymentType paymentType)
        {
            var transaction = await _paymentContext.Transactions
                .Where(t => t.RentalId == rentalId
                         && t.PaymentType == paymentType
                         && t.Status == Status.Pending)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
            return transaction;
        }
        public async Task<IReadOnlyList<Transaction>> GetAllByRentalIdAsync(Guid rentalId, CancellationToken cancellationToken = default)
        {
            return await _paymentContext.Transactions
                .Where(t => t.RentalId == rentalId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<Transaction>> GetPendingByRentalIdAndTypesAsync(Guid rentalId, IEnumerable<PaymentType> paymentTypes, CancellationToken cancellationToken = default)
        {
            return await _paymentContext.Transactions
                .Where(t => t.RentalId == rentalId
                         && paymentTypes.Contains(t.PaymentType)
                         && t.Status == Status.Pending)
                .ToListAsync(cancellationToken);
        }
        public async Task<Transaction?> GetByExternalTokenAsync(string token)
        {
            return await _paymentContext.Transactions
                .FirstOrDefaultAsync(t => t.ExternalToken == token);
        }
        public async Task DeleteAsync(Transaction transaction)
        {
            _paymentContext.Transactions.Remove(transaction);
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            _paymentContext.Transactions.Update(transaction);
        }
    }
}
