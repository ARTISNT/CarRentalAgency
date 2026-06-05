using Contracts.RentalEvents;
using ContractService.Application.Features.Contracts.RenewContract;
using MassTransit;
using MediatR;

namespace ContractService.Infrastructure.Messaging.Consumers;

public class RentalRenewedConsumer(ISender sender) : IConsumer<RentalRenewedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RentalRenewedIntegrationEvent> context)
    {
        var message = context.Message;
        await sender.Send(new RenewContractCommand(message.Id, message.AdditionalPrice, message.NewEndDate));
    }
}