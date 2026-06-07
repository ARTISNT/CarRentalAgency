namespace ContractService.Application.Authorization;

public interface IContractAuthorizationService
{
    public void EnsureCanViewContracts();
    public void EnsureCanCreateContracts(Guid targetClientId);
    public void EnsureCanChangeContractStatus();
}