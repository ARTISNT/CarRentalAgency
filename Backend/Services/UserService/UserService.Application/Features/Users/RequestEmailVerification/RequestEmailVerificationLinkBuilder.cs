using Microsoft.Extensions.Configuration;

namespace UserService.Application.Features.Users.RequestEmailVerification;

public class RequestEmailVerificationLinkBuilder
{
    private readonly string _baseUrl;

    public RequestEmailVerificationLinkBuilder(IConfiguration configuration)
    {
        _baseUrl = configuration["App:VerificationUrl"]
            ?? throw new InvalidOperationException("App:VerificationUrl is not configured.");
    }

    public string Build(string rawToken)
    {
        var separator = _baseUrl.Contains('?') ? '&' : '?';
        return $"{_baseUrl}{separator}token={Uri.EscapeDataString(rawToken)}";
    }
}
