namespace UserService.Application.Features.Users.GetUsers;

public class UserResponse
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public bool EmailVerified { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get;  set; }
    public string Role { get; set; }
}