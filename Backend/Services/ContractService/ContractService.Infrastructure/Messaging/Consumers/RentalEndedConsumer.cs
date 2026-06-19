using Contracts.RentalEvents;
using ContractService.Application.Features.Contracts.EndContract;
using MassTransit;
using MediatR;

namespace ContractService.Infrastructure.Messaging.Consumers;

public class RentalEndedConsumer(ISender sender) : IConsumer<RentalEndedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RentalEndedIntegrationEvent> context)
    {
        var msg = context.Message;
        await sender.Send(new EndContractByRentalCommand(
            msg.RentalId,
            msg.ReturnDate,
            msg.Mileage,
            msg.FuelLevel,
            msg.PenaltyAmount,
            msg.DamageDescription));
    }
}
