using Contracts.ContractEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace RentalService.Infrastructure.Messaging.Consumers;

public class ContractEndedConsumer(ILogger<ContractEndedConsumer> logger)
    : IConsumer<ContractEndedIntegrationEvent>
{
    public Task Consume(ConsumeContext<ContractEndedIntegrationEvent> context)
    {
        var msg = context.Message;
        logger.LogInformation(
            "Return act confirmed for rental {RentalId}: contract {ContractId} ended at {EndedAt}",
            msg.RentalId, msg.ContractId, msg.EndedAt);
        return Task.CompletedTask;
    }
}
