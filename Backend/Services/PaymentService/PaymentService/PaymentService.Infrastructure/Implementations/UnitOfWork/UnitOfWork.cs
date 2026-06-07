using PaymentService.Application.Abstractions.Repositories;
using PaymentService.Application.Abstractions.UnitOfWork;
using PaymentService.Infrastructure.Persistence.DB;

namespace PaymentService.Infrastructure.Implementations.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PaymentContext _paymentContext;
        public ITransactionRepository Transactions { get; }

        public UnitOfWork(PaymentContext paymentContext, ITransactionRepository transactionRepository)
        {
            _paymentContext = paymentContext;
            Transactions = transactionRepository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _paymentContext.SaveChangesAsync(cancellationToken);
        }
    }
}
