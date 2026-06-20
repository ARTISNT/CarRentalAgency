using MediatR;

namespace UserService.Application.Features.Users.VerifyEmail;

public enum EmailVerificationResult
{
    Success,
    AlreadyVerified,
    InvalidToken,
    ExpiredToken,
    UserNotFound
}

public record VerifyEmailQuery(string Token) : IRequest<EmailVerificationResult>;
