using MediatR;

namespace UserService.Application.Features.Users.RemoveUsers;

public record RemoveUserCommand(Guid Id) : IRequest;