namespace RentalService.Application.Exceptions;

public class PassportRequiredException : Exception
{
    public PassportRequiredException(string message) : base(message) { }
}
