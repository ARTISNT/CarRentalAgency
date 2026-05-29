namespace ContractService.Domain.Exceptions.Contracts;

public class ContractNotFoundException : Exception
{
    public ContractNotFoundException()
    {
    }
    public ContractNotFoundException(string message) : base(message)
    {}
    
    public ContractNotFoundException(string message, Exception inner) : base(message, inner)
    {} 
}