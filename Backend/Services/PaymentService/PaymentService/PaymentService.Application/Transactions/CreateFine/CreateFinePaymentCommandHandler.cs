using MediatR;
using PaymentService.Application.Abstractions.Clients;
using PaymentService.Application.Abstractions.PaymentGateway;
using PaymentService.Application.Abstractions.UnitOfWork;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Application.Transactions.CreateFine
{
    public class CreateFinePaymentCommandHandler : IRequestHandler<CreateFinePaymentCommand, string>
    {
        private readonly IRentalServiceClient _rentalClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;

        public CreateFinePaymentCommandHandler(IUnitOfWork unitOfWork, IRentalServiceClient rentalClient, IPaymentGateway paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _rentalClient = rentalClient;
            _paymentGateway = paymentGateway;
        }

        public async Task<string> Handle(CreateFinePaymentCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                throw new ArgumentException("Fine amount must be positive");

            var rental = await _rentalClient.GetRentalByIdAsync(request.RentalId);

            if (request.Amount > rental.FineOutstanding)
            {
                throw new InvalidOperationException(
                    $"Amount {request.Amount} exceeds outstanding fine {rental.FineOutstanding}");
            }

            var paymentData = await _paymentGateway.CreateSessionsAsync(
                request.Amount,
                rental.RentalId.ToString(),
                "Оплата штрафа по аренде");

            var transaction = new Transaction(
                request.Amount,
                paymentData.Token,
                PaymentConstants.CardId,
                request.RentalId,
                PaymentType.Fine,
                request.Reason);

            await _unitOfWork.Transactions.CreateAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return paymentData.RedirectUrl;
        }
    }
}
