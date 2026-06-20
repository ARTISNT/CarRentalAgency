using MediatR;
using UserService.Application.Common;
using UserService.Application.Features.Users.RequestEmailVerification;
using UserService.Domain.DomainEvents;

namespace UserService.Application.Features.Users.RegisterUser;

public class RegisterUserDomainEventHandler(ISender sender) : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public async Task HandleAsync(UserRegisteredDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await sender.Send(new RequestEmailVerificationCommand(domainEvent.Id), cancellationToken);
    }
}
