using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Common;
using UserService.Application.Features.Users.LoginUser;
using UserService.Domain.Users;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace UserService.Infrastructure.Services;

public sealed class UserJwtProvider(IConfiguration configuration) : IJwtProvider
{
    public string CreateJwtToken(User user)
    {
        var secureKey = configuration["UserJwt:SecretKey"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secureKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
                new Claim("role", user.Role.ToString()),
                new Claim("email_verified", user.EmailVerified ? "true" : "false"),
            ]),
            Claims = new Dictionary<string, object>()
            {
                {"permissions", user.Role.Permissions.Select(p => p.ToString()).ToList()},
            },
            Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("UserJwt:ExpireMinutes")),
            SigningCredentials =  credentials,
            Issuer = configuration["UserJwt:Issuer"],
            Audience = configuration["UserJwt:Audience"],
        };

        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return token;
    }
}