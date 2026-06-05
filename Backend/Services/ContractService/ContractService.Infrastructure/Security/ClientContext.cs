using System.Security.Claims;
using ContractService.Application.Abstractions.Security;
using Microsoft.AspNetCore.Http;

namespace ContractService.Infrastructure.Security;

public class ClientContext(IHttpContextAccessor accessor) : IClientContext
{
    public Guid ClientId =>
        Guid.Parse(
            accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!
        );

    public string[] Permissions => accessor.HttpContext?.User
        .FindAll("permissions")
        .Select(c => c.Value)
        .ToArray() ?? Array.Empty<string>();
}