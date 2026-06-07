using PaymentService.Application.Abstractions.Clients;
using PaymentService.Application.Abstractions.PaymentGateway;
using PaymentService.Application.Abstractions.UnitOfWork;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;
using MediatR;

namespace PaymentService.Application.Transactions.Create
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, string>
    {
        private readonly IRentalServiceClient _rentalClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;

        public CreateTransactionCommandHandler(IUnitOfWork unitOfWork, IRentalServiceClient rentalClient, IPaymentGateway paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _rentalClient = rentalClient;
            _paymentGateway = paymentGateway;
        }

        public async Task<string> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var paymentType = Domain.ValueObjects.PaymentType.FromName(request.PaymentType, ignoreCase: true);

            var existingTransaction = await _unitOfWork.Transactions.GetByRentalIdAndTypeAsync(request.RentalId, paymentType);

            if (existingTransaction is not null && existingTransaction.CreatedAt > DateTime.UtcNow.AddMinutes(30))
                return _paymentGateway.BuildUrl(existingTransaction.ExternalToken);

            var rental = await _rentalClient.GetRentalByIdAsync(request.RentalId);

            var amount = paymentType == Domain.ValueObjects.PaymentType.Deposit
                ? rental.DepositAmount
                : rental.TotalPrice;

            var paymentData = await _paymentGateway.CreateSessionsAsync(amount, rental.RentalId.ToString());
            var transaction = new Transaction(amount, paymentData.Token, PaymentConstants.CardId, request.RentalId, paymentType);

            await _unitOfWork.Transactions.CreateAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return paymentData.RedirectUrl;
        }
    }
}
