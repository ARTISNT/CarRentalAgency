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

    public bool? IsActive
    {
        get
        {
            var claim = accessor.HttpContext?.User.FindFirst("is_active")?.Value;
            return claim is null ? null : claim == "true";
        }
    }
}
