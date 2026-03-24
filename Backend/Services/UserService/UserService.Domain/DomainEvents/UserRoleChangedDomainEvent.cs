using MediatR;
using UserService.Domain.Roles;
using UserService.Domain.Users;

namespace UserService.Domain.DomainEvents;

public record UserRoleChangedDomainEvent(Guid Id, string NewRole, DateTime OccuredOn) : INotification;