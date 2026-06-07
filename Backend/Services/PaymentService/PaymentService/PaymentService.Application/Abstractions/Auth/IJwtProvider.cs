namespace PaymentService.Application.Abstractions.Auth;

public interface IJwtProvider
{
    string GenerateServiceToken(string serviceName, params string[] scopes);
}
