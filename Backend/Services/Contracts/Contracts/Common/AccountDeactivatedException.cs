namespace Contracts.Common;

public class AccountDeactivatedException : Exception
{
    public AccountDeactivatedException() : base("Account is deactivated.") { }

    public AccountDeactivatedException(string message) : base(message) { }
}
