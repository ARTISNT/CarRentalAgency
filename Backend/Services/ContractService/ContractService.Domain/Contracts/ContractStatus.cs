using ContractService.Domain.Common;

namespace ContractService.Domain.Contracts;

public class ContractStatus : Enumeration
{
    public static readonly ContractStatus Active = new ContractStatus(1, "Active");
    public static readonly ContractStatus Ended = new ContractStatus(2, "Ended");
    public static readonly ContractStatus AwaitingSignature = new ContractStatus(3, "AwaitingSignature");
    
    public ContractStatus(int id, string name) : base(id, name)
    {
    }
}