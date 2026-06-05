namespace ContractService.Domain.Exceptions.Contracts;

public class ContractTemplateNotFoundException : Exception
{
    public ContractTemplateNotFoundException()
    {
    }
    public ContractTemplateNotFoundException(string message) : base(message)
    {}

    public ContractTemplateNotFoundException(string message, Exception inner) : base(message, inner)
    {}
}