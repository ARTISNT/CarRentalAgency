using Contracts.ContractEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using RentalService.Domain.Rentals;

namespace RentalService.Infrastructure.Messaging.Consumers;

public class ContractSignedConsumer(
    IRentalRepository rentalRepository,
    ILogger<ContractSignedConsumer> logger)
    : IConsumer<ContractSignedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ContractSignedIntegrationEvent> context)
    {
        var msg = context.Message;

        var rental = await rentalRepository.GetRentalAsync(msg.RentalId, context.CancellationToken);
        if (rental is null)
        {
            logger.LogWarning("Rental {RentalId} not found for contract signed event", msg.RentalId);
            return;
        }

        rental.MarkContractSigned(msg.SignedAt);

        if (rental.DepositPaidAt.HasValue)
        {
            rental.StartRental();
            logger.LogInformation("Rental {RentalId} started after contract signed and deposit paid", msg.RentalId);
        }
        else
        {
            logger.LogInformation("Rental {RentalId} contract signed, waiting for deposit payment", msg.RentalId);
        }

        await rentalRepository.UpdateRentalAsync(rental, context.CancellationToken);
    }
}
