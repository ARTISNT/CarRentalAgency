using MediatR;

namespace UserService.Application.Features.Users.RequestEmailVerification;

public record RequestEmailVerificationCommand(Guid UserId) : IRequest;
