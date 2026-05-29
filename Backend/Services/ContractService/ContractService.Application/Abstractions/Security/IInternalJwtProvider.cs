namespace ContractService.Application.Abstractions.Security;

public interface IInternalJwtProvider
{
    public string GenerateServiceToken(string serviceName, params string[] scopes);
}