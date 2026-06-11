using Contracts.RentalEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace RentalService.Infrastructure.Messaging.Consumers;

public class ContractCreationFaultConsumer(
    ILogger<ContractCreationFaultConsumer> logger)
    : IConsumer<Fault<RentalCreatedIntegrationEvent>>
{
    public Task Consume(ConsumeContext<Fault<RentalCreatedIntegrationEvent>> context)
    {
        var fault = context.Message;
        var originalMessage = fault.Message;

        logger.LogError(
            "Contract creation failed for rental {RentalId}, user {UserId}, car {CarId}. Rental left in AwaitingConfirmation for manual handling. Fault: {Exceptions}",
            originalMessage.RentalId,
            originalMessage.UserId,
            originalMessage.CarId,
            string.Join("; ", fault.Exceptions.Select(e => $"{e.ExceptionType}: {e.Message}")));

        return Task.CompletedTask;
    }
}
