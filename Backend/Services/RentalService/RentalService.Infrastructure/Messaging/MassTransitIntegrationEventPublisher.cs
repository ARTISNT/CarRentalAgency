using MassTransit;
using RentalService.Application.Common;

namespace RentalService.Infrastructure.Messaging;

public class MassTransitIntegrationEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    public Task Publish<T>(T @event, CancellationToken ct)
    {
        return publishEndpoint.Publish(@event, ct);
    }
}