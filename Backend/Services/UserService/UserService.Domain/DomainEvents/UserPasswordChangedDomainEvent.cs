using MediatR;
using UserService.Domain.Users;

namespace UserService.Domain.DomainEvents;

public record UserPasswordChangedDomainEvent(Guid Id, string NewPasswordHash, DateTime OccuredOn) :  INotification;