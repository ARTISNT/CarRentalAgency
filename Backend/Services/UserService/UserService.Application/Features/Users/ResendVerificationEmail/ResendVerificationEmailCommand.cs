using MediatR;

namespace UserService.Application.Features.Users.ResendVerificationEmail;

public enum ResendVerificationEmailResult
{
    Sent,
    AlreadyVerified,
    UserNotFound
}

public record ResendVerificationEmailCommand(string Email) : IRequest<ResendVerificationEmailResult>;
