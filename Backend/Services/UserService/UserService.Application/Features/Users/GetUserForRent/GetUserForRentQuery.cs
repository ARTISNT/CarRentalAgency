using Contracts.Common;
using MediatR;
using UserService.Application.Common;

namespace UserService.Application.Features.Users.GetUserForRent;

public record GetUserForRentQuery(Guid Id) : IRequest<UserRentInfoResponse>, IAuthorizedRequest;