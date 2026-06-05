using ContractService.Application.Abstractions.Security;
using ContractService.Application.Common;

namespace ContractService.Application.Authorization;

public class ContractAuthorizationPolicy(IClientContext clientContext) : IContractAuthorizationPolicy
{
    public bool CanCreateContract(Guid targetClientId)
    {
        var isOtherClient = targetClientId != clientContext.ClientId;

        if (isOtherClient)
            return HasPermission(Permissions.CreateContractsForOthers);

        return HasPermission(Permissions.CreateContracts);
    }

    public bool CanViewClientContracts()
    {
        return HasPermission(Permissions.ViewAllContracts);
    }

    private bool HasPermission(string permission)
    {
        return clientContext.Permissions.Contains(permission);
    }
}