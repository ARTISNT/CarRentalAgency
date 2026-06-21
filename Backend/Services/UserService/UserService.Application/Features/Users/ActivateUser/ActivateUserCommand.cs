using Contracts.Common;
using MediatR;
using UserService.Application.Common;

namespace UserService.Application.Features.Users.ActivateUser;

public record ActivateUserCommand(Guid Id) : IRequest, IAuthorizedRequest;
