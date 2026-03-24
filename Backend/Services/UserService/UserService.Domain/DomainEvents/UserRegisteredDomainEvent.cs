using MediatR;
using UserService.Domain.Users;

namespace UserService.Domain.DomainEvents;

public record UserRegisteredDomainEvent(Guid Id, DateTime OccuredOn) : INotification;
