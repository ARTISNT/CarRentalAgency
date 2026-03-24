using MediatR;
using UserService.Domain.Users;

namespace UserService.Domain.DomainEvents;

public record UserDeactivatedDomainEvent(Guid Id, DateTime OccuredOn) : INotification;
