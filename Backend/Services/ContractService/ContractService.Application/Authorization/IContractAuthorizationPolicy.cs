namespace ContractService.Application.Authorization;

public interface IContractAuthorizationPolicy
{
    public bool CanCreateContract(Guid targetClientId);
    public bool CanViewClientContracts();
    
}