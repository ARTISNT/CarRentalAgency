namespace Contracts.UserEvents;

public record EmailVerificationRequestedIntegrationEvent(
    Guid UserId,
    string Email,
    string VerificationLink,
    DateTime OccurredAt);
