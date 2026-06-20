using System.Security.Cryptography;
using System.Text.Json;
using Contracts.UserEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Application.Abstractions;
using UserService.Application.EmailOutbox;
using UserService.Application.Exceptions;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.RequestEmailVerification;

public class RequestEmailVerificationCommandHandler(
    IUserRepository userRepository,
    IEmailVerificationTokenHasher hasher,
    IEmailOutboxRepository outboxRepository,
    IUnitOfWork unitOfWork,
    RequestEmailVerificationLinkBuilder linkBuilder,
    ILogger<RequestEmailVerificationCommandHandler> logger)
    : IRequestHandler<RequestEmailVerificationCommand>
{
    private const int TokenSizeBytes = 32;
    private const int TokenLifetimeHours = 24;
    private const string EventType = "EmailVerificationRequested";

    public async Task Handle(RequestEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new UserNotFoundException($"User {request.UserId} not found.");

        if (user.EmailVerified)
        {
            logger.LogInformation("Skip email verification request for already verified user {UserId}", user.Id);
            return;
        }

        var rawToken = GenerateSecureToken();
        var tokenHash = hasher.Hash(rawToken);
        var now = DateTime.UtcNow;
        var expiresAt = now.AddHours(TokenLifetimeHours);

        user.RequestEmailVerification(tokenHash, expiresAt, now);
        userRepository.Update(user);

        var verificationLink = linkBuilder.Build(rawToken);

        var payload = new EmailVerificationRequestedIntegrationEvent(
            user.Id,
            user.Email.Value,
            verificationLink,
            now);

        var outboxEntry = new EmailOutboxEntry
        {
            Id = Guid.NewGuid(),
            EventType = EventType,
            UserId = user.Id,
            Email = user.Email.Value,
            VerificationLink = verificationLink,
            PayloadJson = JsonSerializer.Serialize(payload),
            CreatedAt = now,
            Attempts = 0,
            NextAttemptAt = now,
        };

        outboxRepository.Add(outboxEntry);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Email verification requested for user {UserId}", user.Id);
    }

    private string GenerateSecureToken()
    {
        var buffer = new byte[TokenSizeBytes];
        RandomNumberGenerator.Fill(buffer);
        return Base64UrlEncode(buffer);
    }

    private static string Base64UrlEncode(byte[] bytes)
        => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
