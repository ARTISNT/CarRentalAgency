using RentalService.Domain.Common;

namespace RentalService.Application.Common;

public interface IDomainEventHandler<in T> where T : IDomainEvent
{
    public Task HandleAsync(T domainEvent, CancellationToken cancellationToken);
}
