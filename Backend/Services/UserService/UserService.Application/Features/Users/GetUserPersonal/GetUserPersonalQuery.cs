using Contracts.Common;
using MediatR;
using UserService.Application.Common;
using UserService.Application.Features.Users.GetUsers;

namespace UserService.Application.Features.Users.GetUserPersonal;

public record GetUserPersonalQuery(Guid Id) : IRequest<UserResponseWithPassport>, IAuthorizedRequest;