namespace UserService.Domain.Users;

public class InvalidEmailVerificationTokenException : Exception
{
    public InvalidEmailVerificationTokenException() : base("Email verification token is invalid.")
    {
    }

    public InvalidEmailVerificationTokenException(string message) : base(message)
    {
    }
}
