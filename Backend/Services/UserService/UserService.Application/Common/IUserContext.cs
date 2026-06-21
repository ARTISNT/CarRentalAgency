namespace UserService.Application.Common;

public interface IUserContext
{
    Guid? UserId { get; }
    bool IsUserRequest { get; }
}
