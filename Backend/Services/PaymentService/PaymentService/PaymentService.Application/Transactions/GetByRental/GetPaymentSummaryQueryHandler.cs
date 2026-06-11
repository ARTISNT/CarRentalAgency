using MediatR;
using PaymentService.Application.Abstractions.Clients;
using PaymentService.Application.Abstractions.UnitOfWork;
using PaymentService.Application.DTOs.Rentals;

namespace PaymentService.Application.Transactions.GetByRental
{
    public class GetPaymentSummaryQueryHandler : IRequestHandler<GetPaymentSummaryQuery, PaymentSummaryDto>
    {
        private readonly IRentalServiceClient _rentalClient;
        private readonly IUnitOfWork _unitOfWork;

        public GetPaymentSummaryQueryHandler(IRentalServiceClient rentalClient, IUnitOfWork unitOfWork)
        {
            _rentalClient = rentalClient;
            _unitOfWork = unitOfWork;
        }

        public async Task<PaymentSummaryDto> Handle(GetPaymentSummaryQuery request, CancellationToken cancellationToken)
        {
            var rental = await _rentalClient.GetRentalByIdAsync(request.RentalId);
            var transactions = await _unitOfWork.Transactions.GetAllByRentalIdAsync(request.RentalId, cancellationToken);

            var transactionDtos = transactions
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

            return new PaymentSummaryDto(
                rental.RentalId,
                rental.TotalPrice,
                rental.DepositAmount,
                rental.PaidAmount,
                rental.RequiredAmount,
                rental.RemainingAmount,
                rental.FineOutstanding,
                rental.AdditionalOutstanding,
                rental.PaymentStatus,
                transactionDtos);
        }
    }
}
