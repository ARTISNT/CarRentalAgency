namespace RentalService.Application.Common;

public interface IIntegrationEventPublisher
{
    public Task Publish<T>(T @event, CancellationToken ct);
}