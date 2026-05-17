using UserService.Application.Features.Users.LoginUser;
using UserService.Domain.Users;

namespace UserService.Application.Common;

public interface IJwtProvider
{
    string CreateJwtToken(User user);
}