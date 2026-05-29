namespace UserService.Application.Authorization;

public interface IUserAuthorizationService
{
    Task EnsureCanViewUser(Guid targetUserId, CancellationToken cancellationToken = default);
    Task EnsureCanEditUser(Guid targetUserId, CancellationToken cancellationToken = default);
    Task EnsureCanDeactivateUser(Guid targetUserId, CancellationToken cancellationToken = default);
    Task EnsureCanActivateUser(Guid targetUserId, CancellationToken cancellationToken = default);
}