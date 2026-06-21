using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using UserService.Application.Common;

namespace UserService.Infrastructure.Services;

public class UserContext(IHttpContextAccessor accessor) : IUserContext
{
    public Guid? UserId
    {
        get
        {
            var sub = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public bool IsUserRequest
    {
        get
        {
            var sub = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(sub, out _);
        }
    }
}
