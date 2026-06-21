using Contracts.Common;
using MediatR;
using UserService.Application.Common;

namespace UserService.Application.Features.Users.DeactivateUser;

public record DeactivateUserCommand(Guid Id) : IRequest, IAuthorizedRequest;