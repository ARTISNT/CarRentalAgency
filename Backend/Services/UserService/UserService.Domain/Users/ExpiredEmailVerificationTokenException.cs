namespace UserService.Domain.Users;

public class ExpiredEmailVerificationTokenException : Exception
{
    public ExpiredEmailVerificationTokenException() : base("Email verification token has expired.")
    {
    }

    public ExpiredEmailVerificationTokenException(string message) : base(message)
    {
    }
}
