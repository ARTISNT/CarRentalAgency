using PaymentService.Application.Abstractions.Repositories;

namespace PaymentService.Application.Abstractions.UnitOfWork
{
    public interface IUnitOfWork
    {
        ITransactionRepository Transactions { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
