using MediatR;

namespace UserService.Application.Features.Users.GetUserForRent;

public record GetUserForRentQuery(Guid Id) : IRequest<UserRentInfoResponse>;