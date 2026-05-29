using System.Security.Claims;
using ContractService.Application.Abstractions.Security;
using Microsoft.AspNetCore.Http;

namespace ContractService.Infrastructure.Security;

public class UserContext(IHttpContextAccessor accessor) : IUserContext
{
    public Guid UserId =>
        Guid.Parse(
            accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!
        );
}