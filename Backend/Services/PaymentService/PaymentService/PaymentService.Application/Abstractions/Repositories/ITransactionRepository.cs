using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Application.Abstractions.Repositories
{
    public interface ITransactionRepository
    {
        Task CreateAsync(Transaction transaction);
        Task<Transaction?> GetByIdAsync(Guid id);
        Task<Transaction?> GetByRentalIdAsync(Guid rentalId);
        Task<Transaction?> GetByExternalTokenAsync(string token);
        Task<Transaction?> GetByTrackingIdAsync(string trackingId);
        Task<Transaction?> GetByRentalIdAndTypeAsync(Guid rentalId, PaymentType paymentType);
        Task<Transaction?> GetCompletedByRentalIdAndTypeAsync(Guid rentalId, PaymentType paymentType);
        Task<IReadOnlyList<Transaction>> GetAllByRentalIdAsync(Guid rentalId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Transaction>> GetPendingByRentalIdAndTypesAsync(Guid rentalId, IEnumerable<PaymentType> paymentTypes, CancellationToken cancellationToken = default);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(Transaction transaction);
    }
}
