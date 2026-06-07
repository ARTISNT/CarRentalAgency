using Contracts.RentalEvents;
using ContractService.Application.Features.Contracts.CancelContract;
using MassTransit;
using MediatR;

namespace ContractService.Infrastructure.Messaging.Consumers;

public class RentalCancelledConsumer(ISender sender) : IConsumer<RentalCancelledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RentalCancelledIntegrationEvent> context)
    {
        var msg = context.Message;
        await sender.Send(new CancelContractByRentalCommand(msg.RentalId, msg.Reason));
    }
}
