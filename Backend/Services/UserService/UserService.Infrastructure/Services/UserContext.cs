using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using UserService.Application.Common;

namespace UserService.Infrastructure.Services;

public class UserContext(IHttpContextAccessor accessor) : IUserContext
{
    public Guid UserId =>
        Guid.Parse(
            accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!
        );
}