using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PaymentService.Application.Abstractions.Auth;

namespace PaymentService.Infrastructure.Implementations.Auth;

public sealed class InternalJwtProvider(IConfiguration configuration) : IJwtProvider
{
    public string GenerateServiceToken(string serviceName, params string[] scopes)
    {
        var claims = new List<Claim>
        {
            new Claim("service", serviceName)
        };

        foreach (var scope in scopes)
        {
            claims.Add(new Claim("scope", scope));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["InternalJwt:SecretKey"]));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["InternalJwt:Issuer"],
            audience: configuration["InternalJwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("InternalJwt:ExpireMinutes")),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}
