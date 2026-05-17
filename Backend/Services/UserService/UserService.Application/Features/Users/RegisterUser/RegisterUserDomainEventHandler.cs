using UserService.Application.Common;
using UserService.Domain.DomainEvents;

namespace UserService.Application.Features.Users.RegisterUser;

public class RegisterUserDomainEventHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public Task HandleAsync(UserRegisteredDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}