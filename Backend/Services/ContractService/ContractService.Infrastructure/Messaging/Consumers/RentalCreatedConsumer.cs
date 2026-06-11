using Contracts.RentalEvents;
using ContractService.Application.Features.Contracts.CreateContract;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContractService.Infrastructure.Messaging.Consumers;

public class RentalCreatedConsumer(
    ISender sender,
    ILogger<RentalCreatedConsumer> logger)
    : IConsumer<RentalCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RentalCreatedIntegrationEvent> context)
    {
        var msg = context.Message;

        try
        {
            await sender.Send(new CreateContractCommand(
                msg.UserId,
                msg.RentalId,
                msg.CarId));

            logger.LogInformation(
                "Contract created for rental {RentalId}, user {UserId}, car {CarId}",
                msg.RentalId, msg.UserId, msg.CarId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to create contract for rental {RentalId}, user {UserId}, car {CarId}: {ErrorMessage}",
                msg.RentalId, msg.UserId, msg.CarId, ex.Message);
            throw;
        }
    }
}
