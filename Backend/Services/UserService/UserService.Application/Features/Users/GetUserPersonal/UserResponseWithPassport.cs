using UserService.Application.Features.Users.GetUsers;

namespace UserService.Application.Features.Users.GetUserPersonal;

public class UserResponseWithPassport : UserResponse
{
    public PassportDto? PassportDto { get; set; }
}