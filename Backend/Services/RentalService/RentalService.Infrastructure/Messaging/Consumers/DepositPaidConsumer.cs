using Contracts.PaymentEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using RentalService.Domain.Rentals;

namespace RentalService.Infrastructure.Messaging.Consumers;

public class DepositPaidConsumer(
    IRentalRepository rentalRepository,
    ILogger<DepositPaidConsumer> logger)
    : IConsumer<DepositPaidIntegrationEvent>
{
    public async Task Consume(ConsumeContext<DepositPaidIntegrationEvent> context)
    {
        var msg = context.Message;

        var rental = await rentalRepository.GetRentalAsync(msg.RentalId, context.CancellationToken);
        if (rental is null)
        {
            logger.LogWarning("Rental {RentalId} not found for deposit paid event", msg.RentalId);
            return;
        }

        rental.MarkDepositPaid(msg.PaidAt);

        if (rental.ContractSignedAt.HasValue)
        {
            rental.StartRental();
            logger.LogInformation("Rental {RentalId} started after deposit paid and contract signed", msg.RentalId);
        }
        else
        {
            logger.LogInformation("Rental {RentalId} deposit paid, waiting for contract signing", msg.RentalId);
        }

        await rentalRepository.UpdateRentalAsync(rental, context.CancellationToken);
    }
}
