namespace CarService.Domain.Exceptions;

public class CarDomainException : Exception
{
    public CarDomainException()
    { }

    public CarDomainException(string message)
        : base(message)
    { }

    public CarDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}