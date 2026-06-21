using Contracts.Common;
using MediatR;
using UserService.Application.Common;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.GetUsers;

public record GetUsersQuery(UserSpecification UserSpecification) : IRequest<IReadOnlyCollection<UserResponse>>, IAuthorizedRequest;