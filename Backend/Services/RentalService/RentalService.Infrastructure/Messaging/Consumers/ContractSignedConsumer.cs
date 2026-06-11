using Contracts.ContractEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using RentalService.Domain.Rentals;
using RentalService.Domain.Rentals.Enums;

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
            logger.LogWarning("Rental {RentalId} not found for contract signed event, will retry", msg.RentalId);
            throw new Exception($"Rental {msg.RentalId} not found for contract signed event");
        }

        logger.LogInformation(
            "Processing ContractSignedEvent: Rental {RentalId}, ContractSignedAt before: {ContractSignedAt}, Status: {Status}",
            msg.RentalId, rental.ContractSignedAt, rental.ActivityStatus.Name);

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

        logger.LogInformation(
            "ContractSignedEvent saved: Rental {RentalId}, ContractSignedAt: {ContractSignedAt}, Status: {Status}, DepositPaidAt: {DepositPaidAt}",
            msg.RentalId, rental.ContractSignedAt, rental.ActivityStatus.Name, rental.DepositPaidAt);

        if (!rental.DepositPaidAt.HasValue)
        {
            rental = await rentalRepository.GetRentalAsync(msg.RentalId, context.CancellationToken);
            if (rental is not null
                && rental.DepositPaidAt.HasValue
                && rental.ActivityStatus == RentActivityStatus.AwaitingConfirmation)
            {
                rental.StartRental();
                await rentalRepository.UpdateRentalAsync(rental, context.CancellationToken);
                logger.LogInformation("Rental {RentalId} started after re-read (contract consumer)", msg.RentalId);
            }
        }
    }
}
