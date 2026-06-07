using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RentalService.Application.Abstractions.Security;

namespace RentalService.Infrastructure.Security;

public class ClientContext(IHttpContextAccessor accessor) : IClientContext
{
    public Guid ClientId =>
        Guid.Parse(accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);

    public string[] Permissions => accessor.HttpContext?.User
        .FindAll("permissions")
        .Select(c => c.Value)
        .ToArray() ?? [];
}
