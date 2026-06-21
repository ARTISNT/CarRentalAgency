using Contracts.Common;
using MediatR;
using UserService.Application.Common;

namespace UserService.Application.Features.Users.RemoveUsers;

public record RemoveUserCommand(Guid Id) : IRequest, IAuthorizedRequest;