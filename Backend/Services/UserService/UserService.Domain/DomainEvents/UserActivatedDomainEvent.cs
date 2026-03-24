using MediatR;

namespace UserService.Domain.DomainEvents;

public record UserActivatedDomainEvent(Guid Id, DateTime OccuredOn) : INotification;