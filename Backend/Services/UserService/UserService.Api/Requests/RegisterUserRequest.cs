namespace UserService.Api.Requests;

public class RegisterUserRequest
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}