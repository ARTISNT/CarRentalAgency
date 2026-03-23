using MediatR;
using UserService.Domain.Roles;
using UserService.Domain.Users;

namespace UserService.Domain.DomainEvents;

public record UserRoleWasChanged(User User) : INotification;