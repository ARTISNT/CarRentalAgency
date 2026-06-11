using MediatR;
using PaymentService.Application.Abstractions.Clients;
using PaymentService.Application.Abstractions.PaymentGateway;
using PaymentService.Application.Abstractions.UnitOfWork;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Application.Transactions.CreateAdditional
{
    public class CreateAdditionalPaymentCommandHandler : IRequestHandler<CreateAdditionalPaymentCommand, string>
    {
        private readonly IRentalServiceClient _rentalClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;

        public CreateAdditionalPaymentCommandHandler(IUnitOfWork unitOfWork, IRentalServiceClient rentalClient, IPaymentGateway paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _rentalClient = rentalClient;
            _paymentGateway = paymentGateway;
        }

        public async Task<string> Handle(CreateAdditionalPaymentCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be positive");

            var rental = await _rentalClient.GetRentalByIdAsync(request.RentalId);

            if (request.Amount > rental.AdditionalOutstanding)
            {
                throw new InvalidOperationException(
                    $"Amount {request.Amount} exceeds additional outstanding {rental.AdditionalOutstanding}");
            }

            var paymentData = await _paymentGateway.CreateSessionsAsync(
                request.Amount,
                rental.RentalId.ToString(),
                "Доплата по аренде (продление)");

            var transaction = new Transaction(
                request.Amount,
                paymentData.Token,
                PaymentConstants.CardId,
                request.RentalId,
                PaymentType.Additional,
                request.Reason);

            await _unitOfWork.Transactions.CreateAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return paymentData.RedirectUrl;
        }
    }
}
