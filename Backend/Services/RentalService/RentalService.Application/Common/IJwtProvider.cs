namespace RentalService.Application.Common;

public interface IJwtProvider
{
    public string GenerateServiceToken(string serviceName, params string[] scopes);
}