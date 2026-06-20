namespace UserService.Domain.Users;

public interface IEmailVerificationTokenHasher
{
    string Hash(string rawToken);

    bool Verify(string rawToken, string tokenHash);
}
