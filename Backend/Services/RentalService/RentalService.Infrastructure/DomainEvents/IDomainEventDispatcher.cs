using RentalService.Domain.Common;

namespace RentalService.Infrastructure.DomainEvents;

public interface IDomainEventDispatcher
{
    public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}