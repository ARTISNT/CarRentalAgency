using ContractService.Application.Exceptions;

namespace ContractService.Application.Authorization;

public class ContractAuthorizationService(IContractAuthorizationPolicy contractAuthorizationPolicy) : IContractAuthorizationService
{
    public void EnsureCanViewContracts()
    {
        if(!contractAuthorizationPolicy.CanViewClientContracts())
            throw new ForbiddenException("No permission");
    }
    
    public void EnsureCanCreateContracts(Guid targetClientId)
    {
        if(!contractAuthorizationPolicy.CanCreateContract(targetClientId))
            throw new ForbiddenException("No permission");
    }
}