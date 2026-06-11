using MediatR;
using PaymentService.Application.Abstractions.UnitOfWork;
using PaymentService.Application.DTOs.Rentals;

namespace PaymentService.Application.Transactions.GetByRental
{
    public class GetTransactionsByRentalQueryHandler : IRequestHandler<GetTransactionsByRentalQuery, IEnumerable<PaymentTransactionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTransactionsByRentalQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PaymentTransactionDto>> Handle(GetTransactionsByRentalQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _unitOfWork.Transactions.GetAllByRentalIdAsync(request.RentalId, cancellationToken);

            return transactions
                .Select(t => new PaymentTransactionDto(
                    t.Id,
                    t.Amount,
                    t.PaymentType.Name,
                    t.PaymentMethod?.Name ?? "Card",
                    t.Status.Name,
                    t.ExternalToken,
                    t.Description,
                    t.CreatedAt,
                    t.PaymentDate,
                    t.IsRefunded))
                .ToList();
        }
    }
}
