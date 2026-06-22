using MediatR;
using PaymentService.Application.Abstractions.Clients;
using PaymentService.Application.Abstractions.PaymentGateway;
using PaymentService.Application.Abstractions.UnitOfWork;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Application.Transactions.CreateRemaining
{
    public class CreateRemainingPaymentCommandHandler : IRequestHandler<CreateRemainingPaymentCommand, string>
    {
        private readonly IRentalServiceClient _rentalClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;

        public CreateRemainingPaymentCommandHandler(IUnitOfWork unitOfWork, IRentalServiceClient rentalClient, IPaymentGateway paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _rentalClient = rentalClient;
            _paymentGateway = paymentGateway;
        }

        public async Task<string> Handle(CreateRemainingPaymentCommand request, CancellationToken cancellationToken)
        {
            var pendingRemaining = await _unitOfWork.Transactions.GetPendingByRentalIdAndTypesAsync(
                request.RentalId, new[] { PaymentType.Fine, PaymentType.Additional });

            if (pendingRemaining.Count > 0)
            {
                throw new InvalidOperationException(
                    "Доплата по аренде уже инициирована. Дождитесь завершения или отмените предыдущую попытку.");
            }

            var rental = await _rentalClient.GetRentalByIdAsync(request.RentalId);

            if (rental.ActivityStatus != "Active" && rental.ActivityStatus != "Completed")
                throw new InvalidOperationException(
                    "Cannot pay remaining for a rental that hasn't started yet. Pay the deposit first.");

            if (rental.RemainingAmount <= 0)
                throw new InvalidOperationException("Nothing left to pay");

            var amount = rental.RemainingAmount;

            var paymentData = await _paymentGateway.CreateSessionsAsync(
                amount,
                rental.RentalId.ToString(),
                "Доплата по аренде");

            var paymentType = rental.FineOutstanding > 0
                ? PaymentType.Fine
                : PaymentType.Additional;

            var description = rental.FineOutstanding > 0
                ? "Доплата штрафа"
                : "Доплата по аренде";

            var transaction = new Transaction(
                amount,
                paymentData.Token,
                PaymentConstants.CardId,
                request.RentalId,
                paymentType,
                description);

            await _unitOfWork.Transactions.CreateAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return paymentData.RedirectUrl;
        }
    }
}
