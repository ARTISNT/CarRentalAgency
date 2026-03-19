using MediatR;
using UserService.Domain.Users;

namespace UserService.Domain.Events;

public class UserDeactivatedEvent(User user) : INotification;
