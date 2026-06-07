using PaymentService.Application.Abstractions.PaymentGateway;
using PaymentService.Application.Abstractions.Repositories;
using PaymentService.Application.Abstractions.UnitOfWork;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;
using MediatR;

namespace PaymentService.Application.Transactions.Refund
{
    public class RefundTransactionCommandHandler : IRequestHandler<RefundTransactionCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;

        public RefundTransactionCommandHandler(IUnitOfWork unitOfWork, IPaymentGateway paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
        }

        public async Task Handle(RefundTransactionCommand request, CancellationToken cancellationToken)
        {
            var depositTransaction = await _unitOfWork.Transactions.GetByRentalIdAndTypeAsync(
                request.RentalId, Domain.ValueObjects.PaymentType.Deposit);

            if (depositTransaction is null)
                throw new InvalidOperationException("No successful deposit found for this rental");

            if (depositTransaction.IsRefunded)
                throw new InvalidOperationException("Deposit has already been refunded");

            var refundToken = await _paymentGateway.RefundAsync(
                depositTransaction.ExternalToken,
                depositTransaction.Amount);

            var refundTransaction = new Transaction(
                depositTransaction.Amount,
                refundToken,
                PaymentConstants.CardId,
                request.RentalId,
                Domain.ValueObjects.PaymentType.DepositRefund);

            await _unitOfWork.Transactions.CreateAsync(refundTransaction);

            depositTransaction.MarkRefunded();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
