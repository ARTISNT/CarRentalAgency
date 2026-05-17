using UserService.Domain.Common;

namespace UserService.Application.Common;

public interface IDomainEventHandler<in T> where T : IDomainEvent
{
    public Task HandleAsync(T domainEvent, CancellationToken cancellationToken);
}