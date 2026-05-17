using MediatR;

namespace UserService.Application.Features.Users.AddUserPassport;

public record AddUserPassportCommand(Guid UserId, PassportRequest PassportRequest) : IRequest;