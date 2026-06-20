using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.VerifyEmail;

public class VerifyEmailQueryHandler(
    IUserRepository userRepository,
    IEmailVerificationTokenHasher hasher,
    ILogger<VerifyEmailQueryHandler> logger)
    : IRequestHandler<VerifyEmailQuery, EmailVerificationResult>
{
    public async Task<EmailVerificationResult> Handle(VerifyEmailQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
            return EmailVerificationResult.InvalidToken;

        var user = await userRepository.GetByVerificationTokenHashAsync(
            hasher.Hash(request.Token), cancellationToken);

        if (user is null)
        {
            logger.LogInformation("Email verification token did not match any user");
            return EmailVerificationResult.InvalidToken;
        }

        if (user.EmailVerified)
            return EmailVerificationResult.AlreadyVerified;

        try
        {
            user.ConfirmEmail(request.Token, hasher, DateTime.UtcNow);
        }
        catch (ExpiredEmailVerificationTokenException)
        {
            user.ClearExpiredEmailVerificationToken();
            await userRepository.UpdateAsync(user, cancellationToken);
            return EmailVerificationResult.ExpiredToken;
        }
        catch (InvalidEmailVerificationTokenException)
        {
            return EmailVerificationResult.InvalidToken;
        }

        await userRepository.UpdateAsync(user, cancellationToken);
        logger.LogInformation("Email verified for user {UserId}", user.Id);
        return EmailVerificationResult.Success;
    }
}
